#include <Arduino.h>
#include <WiFi.h>
#include <PubSubClient.h>
#include "bsec.h"

#define LED_BUILTIN 2

void connectToWiFi();
void reconnectMQTT();
void checkIaqSensorStatus();
void errLeds();
String getFormattedMac();

// WiFi credentials
const char* ssid = "neki ssid";
const char* password = "neki pass";

// RabbitMQ (MQTT) config
const char* mqtt_server = "neki ip";
const int mqtt_port = 1883;
const char* mqtt_user = "admin";
const char* mqtt_pass = "admin";
const char* mqtt_topic = "weather/data";

WiFiClient espClient;
PubSubClient client(espClient);

Bsec iaqSensor;
uint8_t bme680I2cAddr = 0x77;
String deviceMac;
char payload[256];

void setup()
{
  Serial.begin(115200);
  delay(1000);
  pinMode(LED_BUILTIN, OUTPUT);

  connectToWiFi();

  client.setServer(mqtt_server, mqtt_port);

  iaqSensor.begin(bme680I2cAddr, Wire);
  checkIaqSensorStatus();
  
  deviceMac = getFormattedMac();
  Serial.println("MAC: " + deviceMac);

  uint8_t nSensors = 8;
  bsec_virtual_sensor_t sensorList[nSensors] = {
    BSEC_OUTPUT_IAQ,
    BSEC_OUTPUT_STATIC_IAQ,
    BSEC_OUTPUT_CO2_EQUIVALENT,
    BSEC_OUTPUT_BREATH_VOC_EQUIVALENT,
    BSEC_OUTPUT_RAW_PRESSURE,
    BSEC_OUTPUT_RAW_GAS,
    BSEC_OUTPUT_SENSOR_HEAT_COMPENSATED_TEMPERATURE,
    BSEC_OUTPUT_SENSOR_HEAT_COMPENSATED_HUMIDITY
  };

  iaqSensor.updateSubscription(sensorList, nSensors, BSEC_SAMPLE_RATE_CONT);
  checkIaqSensorStatus();
}

void loop()
{
  if (!client.connected()) {
    reconnectMQTT();
  }
  client.loop();

  if (iaqSensor.run()) {
    digitalWrite(LED_BUILTIN, LOW);

    snprintf(payload, sizeof(payload),
      "{\"mac\":\"%s\",\"iaq\":%.2f,\"staticIaq\":%.2f,\"co2\":%.2f,\"voc\":%.2f,\"pressure\":%.2f,\"gasResistance\":%.2f,\"temperature\":%.2f,\"humidity\":%.2f}",
      deviceMac.c_str(),
      iaqSensor.iaq,
      iaqSensor.staticIaq,
      iaqSensor.co2Equivalent,
      iaqSensor.breathVocEquivalent,
      iaqSensor.pressure,
      iaqSensor.gasResistance,
      iaqSensor.temperature,
      iaqSensor.humidity
    );

    Serial.println(payload);
    client.publish(mqtt_topic, payload);

    digitalWrite(LED_BUILTIN, HIGH);
  } else {
    checkIaqSensorStatus();
  }
}

void connectToWiFi() {
  Serial.print("Connecting to WiFi...");
  WiFi.begin(ssid, password);
  while (WiFi.status() != WL_CONNECTED) {
    delay(500);
    Serial.print(".");
  }
  Serial.println(" connected!");
}

void reconnectMQTT() {
  while (!client.connected()) {
    Serial.print("Connecting to MQTT...");
    if (client.connect("ESP32Client", mqtt_user, mqtt_pass)) {
      Serial.println(" connected!");
    } else {
      Serial.print(" failed, rc=");
      Serial.print(client.state());
      Serial.println(" retrying in 4s...");
      delay(4000);
    }
  }
}

void checkIaqSensorStatus()
{
  if (iaqSensor.bsecStatus != BSEC_OK) {
    if (iaqSensor.bsecStatus < BSEC_OK) {
      Serial.println("BSEC error code : " + String(iaqSensor.bsecStatus));
      for (;;) errLeds();
    } else {
      Serial.println("BSEC warning code : " + String(iaqSensor.bsecStatus));
    }
  }

  if (iaqSensor.bme68xStatus != BME68X_OK) {
    if (iaqSensor.bme68xStatus < BME68X_OK) {
      Serial.println("BME68X error code : " + String(iaqSensor.bme68xStatus));
      for (;;) errLeds();
    } else {
      Serial.println("BME68X warning code : " + String(iaqSensor.bme68xStatus));
    }
  }
}

void errLeds()
{
  pinMode(LED_BUILTIN, OUTPUT);
  digitalWrite(LED_BUILTIN, HIGH);
  delay(100);
  digitalWrite(LED_BUILTIN, LOW);
  delay(100);
}

String getFormattedMac() {
  // Get raw MAC address as a uint64_t
  uint64_t macRaw = ESP.getEfuseMac();
  
  // Convert raw MAC to a string in HEX
  String macString = String(macRaw, HEX);
  macString.toUpperCase();

  // Format it into XX:XX:XX:XX:XX:XX
  String formattedMac = "";
  for (int i = 0; i < macString.length(); i += 2) {
    formattedMac += macString.substring(i, i + 2);
    if (i < macString.length() - 2) {
      formattedMac += ":";  // Add ":" between each pair of hex characters
    }
  }
  
  return formattedMac;
}

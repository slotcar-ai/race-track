int inByte = 0;    // incoming serial byte
int outByte = 100; // incoming serial byte
int breakByte = 0; // incoming serial byte

void setup()
{
    pinMode(LED_BUILTIN, OUTPUT);

    //Setup Channel B
    pinMode(13, OUTPUT); //Initiates Motor Channel B pin
    pinMode(8, OUTPUT);  //Initiates Brake Channel B pin

    //Motor B forward @ full speed
    digitalWrite(13, HIGH); //Establishes forward direction of Channel B
    digitalWrite(8, LOW);   //Disengage the Brake for Channel B
    analogWrite(11, 0);     //Spins the motor on Channel B at no speed
    // start serial port at 9600 bps:
    Serial.begin(9600);
    while (!Serial)
    {
        ; // wait for serial port to connect. Needed for native USB port only
    }
}

void loop()
{
    digitalWrite(LED_BUILTIN, HIGH); // turn the LED on (HIGH is the voltage level)
    delay(1000);                     // wait for a second
    digitalWrite(LED_BUILTIN, LOW);  // turn the LED off by making the voltage LOW
    delay(1000);
    // if we get a valid byte, read analog ins:
    if (Serial.available() > 1)
    {

        inByte = Serial.read();

        if (inByte == breakByte)
        {
            analogWrite(11, breakByte);
            digitalWrite(8, HIGH); //Engage the Brake for Channel B
        }
        else
        {
            digitalWrite(8, LOW); //Disengage the Brake for Channel B
            analogWrite(11, inByte);
        }
        Serial.write(inByte);
    }
}
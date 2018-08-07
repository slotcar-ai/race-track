int inByte = 0; // incoming serial byte

void setup()
{
    //Setup Channel B
    pinMode(13, OUTPUT); //Initiates Motor Channel A pin
    pinMode(8, OUTPUT);  //Initiates Brake Channel A pin

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

    // if we get a valid byte, read analog ins:
    if (Serial.available() > 0)
    {
        // get incoming byte:
        inByte = Serial.read();
        analogWrite(11, inByte);
        Serial.write(inByte);
    }
}
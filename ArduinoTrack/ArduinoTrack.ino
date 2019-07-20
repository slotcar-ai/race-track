/*
 * Car speed handling
 */
int leftCarSpeed = 0;
int rightCarSpeed = 0;

void updateCarSpeeds() {
  // If not using > 1, two zeroes are read for lcs and rcs. Some stop-bits in action maybe?
  if (Serial.available() > 1) {
    int lcs = Serial.parseInt(SKIP_ALL);
    int rcs = Serial.parseInt(SKIP_ALL);
    leftCarSpeed = asSafeSpeed(lcs);
    rightCarSpeed = asSafeSpeed(rcs);
  }
}

int asSafeSpeed(int unsafeSpeed) {
  if (unsafeSpeed > 100) {
    return 100;
  }
  else if (unsafeSpeed < 0) {
    return 0;
  }
  else {
    return unsafeSpeed;
  }
}

/*
 * Car position handing
 */
int leftCarPosition = 0;
int rightCarPosition = 0;

void readCarPosition() {
  // TODO: Actually read the sensor bus and record the triggered sensor
  leftCarPosition = 1337;
  rightCarPosition = 4242;
}

/*
 * print
 */
const char separator = ',';

void sendCarSpeedAndPosition()
{
  Serial.print(leftCarSpeed, DEC);
  Serial.print(separator);
  Serial.print(rightCarSpeed, DEC);
  Serial.print(separator);
  Serial.print(leftCarPosition, DEC);
  Serial.print(separator);
  Serial.println(rightCarPosition, DEC);
}

/*
 * Setup and looping
 */
void setup() {
  Serial.begin(9600);
}

void loop() {
  updateCarSpeeds();
  readCarPosition();
  sendCarSpeedAndPosition();
  delay(1000);
}

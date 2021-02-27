
#include<Wire.h>
#include<BluetoothSerial.h>
#include <ctype.h>
#include <stdlib.h>
#define RESET 4

BluetoothSerial SerialBT;

typedef enum _Exercise { None = -1, PushUp, Squat, LegRaise } Exercise; // 운동 변수 정의

void accCode();
int checkLimitInput(int input); // 운동 횟수 체크 변수
Exercise checkExerInput(int input); // 운동 종류 체크 함수
void resetVar(); // 변수 초기화

const int MPU_addr = 0x68; // I2 C address of the MPU-6050

int AcX, AcY, AcZ, Tmp, GyX, GyY, GyZ;

Exercise curExer; // 현재 운동
bool didDown; // 내려갔는지 확인
bool oneExerciseDone; // 내려갔다 올라오면 운동 하나 완료를 체크하는 변수

int countLimit; // 유니티에서 보내줄 운동에 대한 변수
int exerCount; // 운동 카운트 변수

const int perpMax[3] = {19000, 19000, 19000};
const int perpMin[3] = {16000, 16000, 16000};
const int measureMax[3] = {7000, 7000, 7000};
const int measureMin[3] = {2000, 2000, 2000};


void setup() {
  
  didDown = false;
  oneExerciseDone = false;
  countLimit = -1;
  exerCount = 0;
  curExer = None;
  
  Wire.begin();

  Wire.beginTransmission(MPU_addr);

  Wire.write(0x6B); // PWR_MGMT_1 register

  Wire.write(0); // set to zero (wakes up the MPU-6050)

  Wire.endTransmission(true);

  Serial.begin(115200);

  SerialBT.begin("ESP32_train");

  Serial.println("the device started, now you can pair it with bluetooth!");
}

void loop() {

  accCode();
   
  if (SerialBT.available()) {
    int input = SerialBT.read();
    Serial.println(input);
    SerialBT.flush();
     if(input == RESET)
     {
        resetVar();
        return;
     }
     
    if (countLimit == -1)
    {
      countLimit = checkLimitInput(input);
    }
    else if(curExer == None) {
      curExer = checkExerInput(input);
    }
   
  }



  if (curExer != None)
  {
    if (AcZ <= perpMax[(int)curExer] && AcZ >= perpMin[(int)curExer]) // 영점(대충 지면과 90도)
    {
     
      if (didDown)
      {
        exerCount++;
        Serial.println(exerCount);
        Serial.println(countLimit);
        SerialBT.print(exerCount);
        didDown = false;
        delay(500);
      }
    }
    else if (AcZ <= measureMax[(int)curExer] && AcZ >= measureMin[(int)curExer]) // 측정값 범위(대충 지면과 15~20도)
    {
      didDown = true;
    }
    
    if (exerCount == countLimit)
    {
      exerCount = 0;
      curExer = None;
      delay(1000);
    }
  }
}
void accCode()
{
  Wire.beginTransmission(MPU_addr);

  Wire.write(0x3B); // starting with register 0x3B (ACCEL_XOUT_H)

  Wire.endTransmission(false);

  Wire.requestFrom(MPU_addr, 14, true); // request a total of 14 registers



  AcX = Wire.read() << 8 | Wire.read(); // 0x3B (ACCEL_XOUT_H) & 0x3C (ACCEL_XOUT_L)

  AcY = Wire.read() << 8 | Wire.read(); // 0x3D (ACCEL_YOUT_H) & 0x3E (ACCEL_YOUT_L)

  AcZ = Wire.read() << 8 | Wire.read(); // 0x3F (ACCEL_ZOUT_H) & 0x40 (ACCEL_ZOUT_L)

  Tmp = Wire.read() << 8 | Wire.read(); // 0x41 (TEMP_OUT_H) & 0x42 (TEMP_OUT_L)

  GyX = Wire.read() << 8 | Wire.read(); // 0x43 (GYRO_XOUT_H) & 0x44 (GYRO_XOUT_L)

  GyY = Wire.read() << 8 | Wire.read(); // 0x45 (GYRO_YOUT_H) & 0x46 (GYRO_YOUT_L)

  GyZ = Wire.read() << 8 | Wire.read(); // 0x47 (GYRO_ZOUT_H) & 0x48 (GYRO_ZOUT_L)



  /* Serial.print("AcX = "); Serial.print(AcX);

  Serial.print(" | AcY = "); Serial.println(AcY);

  SerialBT.print(AcZ);

  Serial.print(" | AcZ = "); Serial.print(AcZ);

  Serial.print(" | Tmp = "); Serial.print(Tmp / 340.00 + 36.53); //equation for temperature in degrees C from datasheet

  Serial.print(" | GyX = "); Serial.print(GyX);

  Serial.print(" | GyY = "); Serial.println(GyY);

  Serial.print(" | GyZ = "); Serial.println(GyZ); */

}
int checkLimitInput(int input) // 입력된 운동 횟수를 체크한다. 10~99
{
  
  if (input <= 0 || input > 100)
    return -1;
    
  
  return input;
}

Exercise checkExerInput(int input) // 운동 변수를 int로 바꾼다 0 ~ 3
  if(input == 4)
  {
    resetVar();
    return None;
  }
    
  return (Exercise) input;
}

void resetVar()
{
  didDown = false;
  oneExerciseDone = false;
  countLimit = -1;
  exerCount = 0;
  curExer = None;
}

/*
측정값 범위 : 모두 통일, 레그레이즈만 방향 다르게 설정(삼각형이 발쪽으로 나머지는 머리방향)
*/

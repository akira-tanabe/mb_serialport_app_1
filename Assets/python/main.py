from microbit import *

# https://www.ma.noda.tus.ac.jp/u/tg/html/scale.html

# while True:
#     name = input('What is your name? ')
#     print('Hello', name)

# while True:
#     print(display.read_light_level())
#     sleep(1000) 
#     print("hello")
#     sleep(1000) 

while True:
    p = 0
    
    if button_a.is_pressed(): p+=1
    if button_b.is_pressed(): p+=2
    if pin_logo.is_touched(): p+=4
    axisx,axisy,axisz = accelerometer.get_values()
    gesture  = accelerometer.current_gesture()
    strength = accelerometer.get_strength()
    gest_histry = accelerometer.get_gestures()
    print("\"press\":{}, \"axisx\":{}, \"axisy\":{}, \"axisz\":{}, \"strength\":{}, \"gesture\":\"{}\", \"gest_histry\":{}".format(p, axisx,axisy,axisz, strength, gesture, list(gest_histry) ))
    # compassx, compassy, compassz = compass.get_x(),compass.get_y(), compass.get_z()
    # compasshead = compass.heading()
    # # print(compasshead)
    # print("\"compassx\":{}, \"compassy\":{}, \"compassz\":{}, \"compasshead\":\"{}\"".format(compassx, compassy, compassz,compasshead ))
    # display.scroll(compass.heading())
    # touch
    pin_logo.set_touch_mode(pin0.CAPACITIVE)
    pin0.set_touch_mode(pin0.CAPACITIVE)
    pin1.set_touch_mode(pin0.CAPACITIVE)
    pin2.set_touch_mode(pin0.CAPACITIVE)
    tc=0 # 0-1023
    if pin_logo.is_touched(): 
        tc = pin0.read_digital()
        print("pin_logo tc:{} {}".format(pin0.read_digital(), pin0.read_analog()))
    if button_a.is_pressed():
        pin = pin_speaker
        print("pin_speaker")
        speed = 1000/4
        power = 5
        stp = 0
        c,d,e,f,g,a,b,v = 0,2,4,5,7,9,11,99
        musicstep = [c,c,g,g,a,a,g,v,f,f,e,e,d,d,c,v]
        while len(musicstep)>stp:
            note = int(2**((12-musicstep[stp]) / 12) * 440)
            # note = int(stp * (440/12))
            if musicstep[stp] is not v:
                pin.write_analog(power)
                pin.set_analog_period_microseconds(note)
            print("({}){}".format(stp,note))
            sleep(int(speed*0.9))
            pin.write_analog(0)
            sleep(int(speed*0.1))
            stp+=1
            if button_b.is_pressed(): break
        print("stop")
        pin.write_analog(0)
        
        # pin0.write_analog(0)
        # pin0.write_digital(0)
    sleep(1000) 



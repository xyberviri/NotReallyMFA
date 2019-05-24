# NotReallyMFA
Need to pass a MFA code on the command line? 

Are you bored and want an insecure way of storing and retrieving TOTP codes?

Need to generate shared key and validate it's TOTP code for your current time?

Do you need to test your MFA/2FA implementation and don’t want to add a bunch of codes to your phone/hardware token?

# NotReallyMFA to the notReallyRescue.

Simply call `NotReallyMFA <BASE32Key>` and it will return the TOTP code for that base32key. 

Don't have a base32key to give to the end user? Simply call `NotReallyMFA` for a randomly generated base 32key.

Have more than 1 base32key? Pass as many as you want and NotReallyMFA will return a TOTP code for each of the base32keys. 

# Should I use this app?

You should use Google Auth on your phone/Android device. If you are really concerned about security, you should use a hardware token. This is really only for testing MFA implementations; you definitely should NOT use this in a production environment. 

# Why did you make this?

I needed to test if my 2FA implementation was working and I didn’t want to use google auth on my phone to store a bunch of codes I was going to have to delete. This allows me to just generate a bunch of keys and validate a bunch of keys. 

That’s like 4 birds with one stone…. 

This virtually allows anything that can call a command line the ability to respond to MFA challenges. I'm certain there are other better options out there. This application allows me to trouble shoot MFA applications and test BASE32 tokens without having to use my phone or a hardware token. 


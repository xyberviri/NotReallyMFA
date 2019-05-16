# NotReallyMFA
Need to to pass a MFA code on the command line? 

Do you have some compliance requirement that "in the spirit of the law" requires a TOTP code to access an api?

Are you bored and want an insecure way of storing and retriveing TOTP codes?

NotReallyMFA to the rescue(NotReally).

Simply call `NotReallyMFA <BASE32Key>` and it will return the TOTP code for that base32key. 

Don't have a base32key to give to the end user? Simply call `NotReallyMFA` for a randomly generated base 32key.

Have more than 1 base32key? Pass as many as you want and NotReallyMFA will return a TOTP code for each of the base32keys. 

# Why did you make this?
I wanted to make an easy way to interact with sensitive data. Even if that way is less secure. 

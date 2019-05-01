# NotReallyMFA
Need to to pass a MFA code on the command line? 

Do you have some compliance requirement that "in the spirit of the law" requires a TOTP code to access an api?

Are you bored and want an insecure way of storing and retriveing TOTP codes?

NotReallyMFA to the rescue.


simply call `NotReallyMFA <BASE32Key>` and it will return the TOTP code for that base32key. 

Have more than 1 base32key? Pass as many as you want and NotReallyMFA will return a TOTP code for each of the base32keys. 

# Why did you make this?
Why do i need so many damn passwords? Why dont we have something better by now? Why is the solution to just create longer passwords and add additional methods of verification. 


I got into an argument about "best practices" and decided to make this to show that the "spirit of the law" and "letter of the law" are two completely different things. 

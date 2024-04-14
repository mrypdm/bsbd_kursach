# https://dba.stackexchange.com/a/284769/288326

New-SelfSignedCertificate `
-Type SSLServerAuthentication `
-Subject "CN=$env:COMPUTERNAME" `
-DnsName "$env:COMPUTERNAME",'localhost.' `
-KeyAlgorithm "RSA" `
-KeyLength 2048 `
-Hash "SHA256" `
-TextExtension "2.5.29.37={text}1.3.6.1.5.5.7.3.1" `
-NotAfter (Get-Date).AddMonths(36) `
-KeySpec KeyExchange `
-Provider "Microsoft RSA SChannel Cryptographic Provider"
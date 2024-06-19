The solution is based on .net Aspire, so it requries
- .net sdk 8.0
- docker desktop
- dev certificate trusted by running ```dotnet dev-certs https --trust```

The solution was implemented with assumption that no strict check for phonenumber is required:
- Phone number has 9 digit.
- Could have 1 to 3 character country code.

Sms and Smtp are written to corresponding sms and smtp folder in webapi project.

It's nicer to apply DDD for user's property validation but since I haven't use EF for a while,
I run into some issue with mapping ValueObject with EF, so the half CQRS solution.
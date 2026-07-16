OpenSource development of the dissociated discordbot.


Style guide - 
1. Pascal case for functions / classes
2. Camel case for variables.
3. Avoid extensively long names
4. Avoid overly generic big classes, short and consise
5. CommandModules is for anything slash command related.
6. Modules is for any "service" which can be used to communicate with the application.


Infrastructure.
The idea behind the code base is the following, it must be easy to keep track of and write on with multiple people. 
Hence as to why a dispatch pattern is the one that will be utilized, this way we can easily multi thread whilst controlling our events and using handlers.
Why are handlers important? when working together on github we must minimalize our time spent on merging, thing being with handlers we can register them dynamically, meaning we can all work in our seperate files.

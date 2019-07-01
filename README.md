# Cake
![](https://cake.s-ul.eu/lbofRWcs)

Welcome to the repo of Cake! Here you can find the source code for this bot.

Cake is a multipurpose bot that is made to create your life a bit easier.

The bot has support for osu! and twitter at the moment and is still being worked on.

This bot is using .Net Core 2.2.0 for the Console side, every libary is created in .Net Standard 2.0. This means the bot will run on most systems running Windows or Linux. Cake requires a SQL Server (2012 or 2016 preferred) to store its data. If you are planning to host this bot for yourself, there will be a config file that you have to insert your botkey and API keys if needed. Everything else is dynamic and can be changed using the bot.

The bot have custom logging that will call back the time the command toke executing, who excuted it and in which guild. With this information we can check if someone is abusing a command and if we can improve it speed or performance wise. 

## Features:

- osu! module with recently played, top, compare scores and user profiles.

- Can run on multiple platforms.

# EchoVibe

EchoVibe is a .NET Blazor web application where people can share “vibes” (posts about mood, comfort, faith, and everyday life), like and comment, follow other users, and connect as a community.

When life feels heavy, stressed, or lonely, EchoVibe is a place to find encouragement and share honest moments.

## Live demo

- **Website:** https://echovibe-xq8u.onrender.com  
- **GitHub:** https://github.com/EmmanuelOwilli/echovibe  

> Note: The free Render plan may sleep after idle time. The first open can take 30–60 seconds.

## Tech stack

- .NET Blazor (Interactive Server)
- ASP.NET Core Identity (register / login / logout)
- Entity Framework Core + SQLite
- Cloudinary (image uploads)
- Bootstrap + custom CSS
- Open-Meteo (weather for Kampala, Uganda)
- Docker + Render (deployment)

## Features

- Register and login with display name
- Create, view, and edit vibes (title, type, description, cover image)
- Like and comment on vibes
- Follow / unfollow users; Following and Followers lists
- Feeds: New, Popular, and Following-related views
- Search users
- Profile edit (bio, country, profile photo)
- Vibe wall posts (text and optional image) with likes and comments
- Forgot / reset password
- Home page with weather and comfort-focused messaging
- Deployed public URL

## How to run locally

### Requirements
 
 ,m 
- .NET 10 SDK
- (Optional) Cloudinary free account for image uploads

### Steps

```bash
git clone https://github.com/EmmanuelOwilli/echovibe.git
cd echovibe
dotnet restore
dotnet ef database update
dotnet run
# PlexClone

This Projects goal is to clone Plex and give certain simlar functionality. This is just a side project and not meant to be actually used.

## Getting Started

These instructions will get you a copy of the project up and running on your local machine for development and testing purposes.

### Installing

```
git clone https://github.com/denniskalpedis/PlexClone.git
```

cd PlexClone

rename 'appsettings' to 'appsettings.json'

```
Mac/Linux:
mv appsettings appsettings.json

Windows:
ren appsettings appsettings.json

```

dotnet restore

dotnet build

dotnet ef migrations add initial

dotnet ef database update

download ffmpeg from https://ffmpeg.zeranoe.com/builds/

extract and put the files from the /bin/ folder into the bin folder inside the PlexClone app.

change settings inside 'appsettings.json'

dotnet run

## Authors

* **Dennis Kalpedis** - *Initial work* - [denniskalpedis](https://github.com/denniskalpedis)

* **Elizabeth Pedley** - *Initial work* - [elizabethpedley](https://github.com/elizabethpedley)

## Acknowledgments

* Plex
* FFMpeg
* OMDB

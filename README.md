# YouTuber

This is just a simple .NET 5 Console App that will ingest a file of YouTube video IDs and download them all using the Apose HTML lib.

This is written on my Mac but should work fine on Windows as well.

You need to at least specify an input which can be a single YouTube URL or a local path to a file with a list of URLs

Put you list of YouTube video URLs in the input list file as single column - one line per URL. The video ID will be extracted from the URL.

There are couple Command Line options
```
-b myshow -i "/Users/jbob/Desktop/YouTuber/videos.txt" -o "/Users/jbob/Desktop/YouTuber/Downloads" -s 7
```

* b - base outfile name like the format myvideo-#.mp4
* i - full input file path to read from OR a single YouTube video URL
* o - local output folder to write videos to
* s - sequence ID to start from which defaults to 1 but if you need to pick up where you left off

Sadly, the Aspose downloader is synchronous with no callback parameters meaning there's no way to get progress on the download as it is happening? It would appear the downloader loads the entire video into memory and then writes it to disk as opposed to streaming it directly to a filestream?

## Running on a Mac

You'll need Visual Studio for Mac or at least .NET installed on your Mac to run this exe. Weirdly, VS for Mac writes the exe with a .dll extension but it still works fine. Build the project and then right-click publish the project to a folder of your choice. Usually using the Release Configuration. Finally, use the dotnet runner with:

```
> dotnet <full_path>/Raydreams.YouTuber.dll -b myoutfilename -i <full_path>/myvideos.txt -s 1 -o <full_path>/DownloadFolder
```

You can change the extension to **.exe** but you will also need to modify the maifest file with that change, which is the **.deps.json** file

```
"runtime": {
          "Raydreams.YouTuber.dll": {}
        }
```

# License

This example is entirely free to use. However, the Aspose libray is not. You will need to consult [their website](https://www.aspose.com/) for licensing terms of use.
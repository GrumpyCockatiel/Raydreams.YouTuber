# YouTuber

This is just a simple .NET 5 Console App that will ingest a file of YouTube video IDs and download them all using the Apose HTML lib.

This is written on my Mac but should work fine on Windows as well. I will add a switch to set the default path at some point, but right now it defaults to the Desktop and if you use this on Windows you'll need to change it to whatever at

```
public static readonly string DesktopPath = Environment.GetFolderPath( Environment.SpecialFolder.DesktopDirectory );
```

Everything defaults to the desktop for now as

```
/YouTuber/<input_list>.txt
/YouTuber/Downloads/
```

Put you list of YouTube video URLs in the input list file as single column - one line per URL. The video ID will be extracted from the URL

There are couple Command Line options
```
-b myshow -i myvideos.txt -s 1
```

* b - base outfile name like the format myvideo-N.mp4
* i - input file to read from
* s - sequence ID to start from which defaults to 1 but if you need to pick up where you left off

# License

This example is entirely free to use. However, the Aspose libray is not. You will need to consult [their website](https://www.aspose.com/) for licensing terms of use.
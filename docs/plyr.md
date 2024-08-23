To use Plyr for video playback in your web application, follow these steps:

Step 1: Include Plyr Library
Include the Plyr CSS and JavaScript files in your HTML file. You can either use a CDN or host the files locally.
<!-- video pylr CSS -->
<link rel="stylesheet" href="https://cdn.plyr.io/3.7.8/plyr.css" />
<link href="~/lib/plyrvideo/plyr.css" rel="stylesheet" />
<!-- Plyr JavaScript -->
 <script src="https://cdn.plyr.io/3.7.8/plyr.js"></script>
 <script src="~/lib/plyrvideo/js/plyr.js"></script>
 <script src="~/lib/plyrvideo/js/plyr.polyfilled.js"></script>
 
Step 2: Create HTML Structure
Create an HTML <video> element with an ID for the Plyr player to target.

<div class="container">
    <h1 class="mt-3 mb-3">Video Player</h1>

    <video id="player" playsinline controls>
        <source src="path_to_video.mp4" type="video/mp4" />
    </video>
</div>
Replace path_to_video.mp4 with the actual path to your video file.

Step 3: Initialize Plyr
Initialize Plyr by adding JavaScript code to configure its behavior.
<script>
    document.addEventListener('DOMContentLoaded', () => {
              const player = new Plyr('#player', {
                controls: ['play', 'progress', 'current-time', 'mute', 'volume', 'settings', 'fullscreen'], // Specify which controls to display
                autoplay: true, // Automatically start playback
                  loop: { // Configure loop behavior
                      active: true, // Enable loop
                      start: 0, // Loop start time (in seconds)
                      end: null // Loop end time (null means end of the video)
                  },
                  volume: 0.5, // Set initial volume (0 to 1)
                  keyboard: { // Configure keyboard shortcuts
                      focused: true, // Allow keyboard shortcuts when player is focused
                      global: true // Allow keyboard shortcuts globally (not just when player is focused)
                  },
                  tooltips: { // Configure tooltips
                      controls: true, // Show tooltips for controls
                      seek: true // Show seek tooltip
                  }
        });
    });
</script>

Configuration Options
Plyr provides various configuration options to customize its behavior. Some of the common options include:

controls: Specify which controls to display.
autoplay: Automatically start playback.
loop: Configure loop behavior.
volume: Set initial volume (0 to 1).
keyboard: Configure keyboard shortcuts.
tooltips: Configure tooltips.
Uncomment and customize the options as needed in the initialization script.

For a full list of configuration options, refer to the Plyr documentation.

Conclusion
Plyr is a versatile and easy-to-use library for integrating video playback into your web application. By following the steps outlined in this documentation, you can quickly set up a customizable video player with Plyr and enhance the user experience with seamless video playback.

For more information and advanced usage, refer to the official Plyr documentation: Plyr Documentation


You can see Views in fileController for better understanding How you should use it .
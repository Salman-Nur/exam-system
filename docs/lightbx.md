To use Lightbox2 in your web application, follow these steps:

Step 1: Include Lightbox2 Library
Include the Lightbox2 JavaScript and CSS files in your HTML file. You can either download the library and host it locally or include it from a CDN.


<!-- LightBox2 CSS -->
<link href="~/lib/lightbox2/lightbox.min.css" rel="stylesheet" />

<!-- jQuery -->
<script src="~/lib/jquery/dist/jquery-min-3-6-0.js"></script>
<!-- LightBox2 JS -->
<script src="~/lib/lightbox2/lightbox.min.js"></script>


  
Step 2: Create HTML Structure
Create HTML elements to display images or videos that you want to showcase using Lightbox2.


<div class="container">
    <div class="row">
        <div class="col-md-4">
            <a href="path_to_image.jpg" data-lightbox="gallery" data-title="Image 1">
                <img src="path_to_thumbnail.jpg" alt="Image 1" class="img-thumbnail" />
            </a>
        </div>
        <!-- Add more images as needed -->
    </div>
</div>
Replace path_to_image.jpg and path_to_thumbnail.jpg with the actual paths to your image and its thumbnail respectively.

Step 3: Initialize Lightbox
Initialize Lightbox2 by adding JavaScript code to configure its behavior.

html
Copy code
<script>
    lightbox.option({
        'albumLabel': 'Image %1 of %2',
        'alwaysShowNavOnTouchDevices': false,
        'disableScrolling': false,
        'fadeDuration': 600,
        'fitImagesInViewport': true,
        'imageFadeDuration': 600,
        'maxHeight': 9999,
        'maxWidth': 9999,
        'positionFromTop': 50,
        'resizeDuration': 700,
        'showImageNumberLabel': true,
        'wrapAround': false
    });
</script>
Configuration Options
Lightbox2 provides various configuration options to customize its behavior. Some of the common options include:

albumLabel: The label format for the image counter.
fadeDuration: The duration of the fade animation (in milliseconds).
maxHeight, maxWidth: Maximum height and width of the displayed images.
showImageNumberLabel: Whether to display the image number label.
wrapAround: Whether to loop through images when reaching the end of the gallery.
For a full list of configuration options, refer to the Lightbox2 documentation.

Conclusion
Lightbox2 is a lightweight and easy-to-use library for creating image galleries and showcasing visual content on your website. By following the steps outlined in this documentation, you can integrate Lightbox2 into your web application and enhance the user experience with beautiful image displays.

For more information and advanced usage, refer to the official Lightbox2 documentation: Lightbox2 Documentation


You can see Views in fileController for better understanding How you should use it .

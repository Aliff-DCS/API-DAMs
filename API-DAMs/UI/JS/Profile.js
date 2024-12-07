
    // JavaScript to show the upload button when a file is selected
    document.getElementById('<%= fileUpload.ClientID %>').addEventListener('change', function () {
        var btnUpload = document.getElementById('<%= btnUploadImage.ClientID %>');
        if (this.files.length > 0) {
            btnUpload.style.display = 'inline-block'; // Show the button
        } else {
            btnUpload.style.display = 'none'; // Hide the button if no file is selected
        }
    });

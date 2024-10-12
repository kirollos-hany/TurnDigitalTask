const showDeleteSwalFunc = (url, id) => {
    Swal.fire({
        title:'Delete Warning!',
        text: 'Once deleted can\'t be restored',
        icon: 'warning',
        showConfirmButton: true,
        showCancelButton: true,
        confirmButtonText: 'Confirm',
        cancelButtonText: 'Close'
    })
        .then((result) => {
            if (result.isConfirmed) {
                deleteFunc(url, id)
            }
        })
}

const deleteFunc = (url, id) => {
    $.ajax({
        url: url + `/${id}`,
        type: 'DELETE',
        success: function (data) {
            window.location.reload()
        },
        error: function (data) {
            console.log(data)
        }
    })
}

const imagePreviewFunc = (fileInputId, imageId) => {
    document.getElementById(fileInputId).addEventListener('change', function(event) {
        const file = event.target.files[0]; // Get the selected file
        const reader = new FileReader(); // Create a FileReader instance

        reader.onload = function(e) {
            const image = document.getElementById(imageId); // Get the image element
            image.src = e.target.result; // Set the image source to the file data
            image.style.display = 'block'; // Make the image visible
        };

        if (file) {
            reader.readAsDataURL(file); // Read the file as a data URL
        }
    });
}


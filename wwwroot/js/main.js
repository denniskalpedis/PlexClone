$(document).ready(function(){
    $('#addLibraryButton').on('click', function(){
        $('#formPopup').show();
    });
    $('#closeForm').on('click', function(){
        $('#formPopup').hide();
    });
});
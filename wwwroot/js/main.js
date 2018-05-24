var allItems;

function toDom(arr){
    $('#moviesDisplay').html('');
    if(arr.length == 0){
        $('#moviesDisplay').html('<h1> No movies match your search</h1>');
    }
    for(var i=0;i<arr.length;i++){
        $('#moviesDisplay').append('<div class="movieContainer col-3"><img src="'+arr[i].poster+'" alt=""><h3>'+arr[i].title+'<h3><p>'+arr[i].year+'<p></div>');
    }}

function debounced(delay, fn) {
    var timerId;
    return function (...args) {
        if (timerId) {
        clearTimeout(timerId);
        }
        timerId = setTimeout(() => {
        fn(...args);
        timerId = null;
        }, delay);
    }
    }
var bounce = debounced(60, toDom);

$(document).ready(function(){
    $('#addLibraryButton').on('click', function(){
        $('#formPopup').show();
    });
    $('#closeForm').on('click', function(){
        $('#formPopup').hide();
    });

    fetch('/JSON/allitems')
      .then(function(res) {
        return res.json();
      }).then(data => allItems = data);

      $('#search').on('keyup', function(){
        var string = $(this).val().toLowerCase();
        console.log(string);
        var filtered = allItems.filter(item => item.title.toLowerCase().includes(string) || item.genre.toLowerCase().includes(string));
        console.log(filtered);
        bounce(filtered);
      });
    
}); 

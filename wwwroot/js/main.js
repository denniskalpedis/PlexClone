var allItems;

function toDom(arr){
    $('#moviesDisplay').html('');
    $('#singleMovieContainer').hide();
    $('#moviesDisplay').show();
    if(arr.length == 0){
        $('#moviesDisplay').html('<h1> No movies match your search</h1>');
    }
    for(var i=0;i<arr.length;i++){
        $('#moviesDisplay').append('<div class="movieContainer col-3" onclick="renderMovie('+arr[i].id +')"><img src="'+arr[i].poster+'" alt=""><h3>'+arr[i].title+'</h3><p>'+arr[i].year+'</p></div>');
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

function renderMovie(movieId){
    var item = allItems.find(item => item.id == movieId );
    $('#moviesDisplay').hide();
    $('#singleMovieContainer').html('');
    $('#singleMovieContainer').append('<p onclick="backLink()">Go Back</p><img src="'+item.poster+'" alt=""><h3>'+item.title+'<h3><p>'+item.year+'<p>');
    $('#singleMovieContainer').show();
}

function backLink(){
    $('#singleMovieContainer').hide();
    $('#moviesDisplay').show();
}

function renderLibrary(libraryId){
    fetch('/JSON/library/'+ libraryId)
      .then(function(res) {
        return res.json();
      }).then(library =>{
        $('#moviesDisplay').html('');
        $('#singleMovieContainer').hide();
        $('#moviesDisplay').show();
        if(library.length == 0){
            $('#moviesDisplay').html('<h1> There are no Movies in this Library</h1>');
        }else{
            for(var i=0;i<library.length;i++){
                $('#moviesDisplay').append('<div class="movieContainer col-3" onclick="renderMovie('+library[i].id +')"><img src="'+library[i].poster+'" alt=""><h3>'+library[i].title+'</h3><p>'+library[i].year+'</p></div>');
            }
        }
      });
    
}

$(document).ready(function(){
    $('#singleMovieContainer').hide();

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

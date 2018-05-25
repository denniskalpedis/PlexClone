var allItems;

function toDom(arr){
    $('#moviesDisplay').html('');
    $('#singleMovieContainer').hide();
    $('#moviesDisplay').show();
    if(arr.length == 0){
        $('#moviesDisplay').html('<h1> No movies match your search</h1>');
    }
    for(var i=0;i<arr.length;i++){
        $('#moviesDisplay').append('<div class="movieContainer col-3 hover" onclick="renderMovie('+arr[i].id +')"><img src="'+arr[i].poster+'" alt=""><h3>'+arr[i].title+'</h3><p>'+arr[i].year+'</p></div>');
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
    $('#singleMovieContainer').append('<div class="left"><p class="hover goBack" onclick="backLink()">< Back</p><img src="'+item.poster+'" alt=""/><h3>'+item.title+'</h3><p>'+item.year+'</p></div><div class="right"><p><span>'+item.plot+'</span></p><p> Actors: <span>'+item.actors+'</span></p><p> Genre: <span>'+item.genre+'</span></p><p>File Runtime: <span>2:33</span><a href="/paly/'+item.id+'"><img class="play" src="Img/play.png"></a></p><p> Movie Length: <span>'+item.runtime+'</span></p><p>Movie Quality: <span>HD, 720p</span></p><p><img class="rotten"src="/Img/rotten-tomatoes.png" alt="rotten tomaotes icon."/> <span>'+item.rottenTomatoesRating+'</span></p><p><img class="rotten"src="/Img/imdb.png" alt="imdb icon."/> <span>'+item.imdbRating+'</span></p></div>');
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
                $('#moviesDisplay').append('<div class="movieContainer col-3 hover" onclick="renderMovie('+library[i].id +')"><img src="'+library[i].poster+'" alt=""><h3>'+library[i].title+'</h3><p>'+library[i].year+'</p></div>');
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
    $('.refresh').hide();
    $('.delete').hide();

    $(".library").hover(
        function() {
            $(this).children('.refresh').show();
            $(this).children('.delete').show();
        }, function() {
            $(this).children('.refresh').hide();
            $(this).children('.delete').hide();
        }
      );

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

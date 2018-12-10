var allItems;
var timeout = null;

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

function renderMovie(movieId, version = 0){
    console.log(version);
    localStorage.setItem("movie", movieId);
    var item = allItems.find(item => item.id == movieId );
    $('#moviesDisplay').hide();
    $('#singleMovieContainer').html('');
    var play;
    if(item.isplayable){
        play = '<a href="/play/'+item.id+'"><img class="play" src="Img/play.png"></a>';
    }else{
        play = '';
    }
    
    // $('#singleMovieContainer').append('<dropdown class="dropleft float-right"><a class="hover goBack d-inline-block" href="#" id="dropdownMenu" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false">...</a><dropdown class="dropdown-menu" aria-labelledby="dropdownMenu"></dropdown></dropdown><br>');
    // $('.dropdown-menu').append('<a class="dropdown-item">Action</a><a class="dropdown-item">Another action</a><a class="dropdown-item">Something else here</a>');
    $('#singleMovieContainer').append('<p class="hover goBack d-inline-block" onclick="backLink()">< Back</p><div class="dropleft float-right"><a class="hover goBack d-inline-block" href="#" id="dropdownMenu" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false">...</a><dropdown class="dropdown-menu" aria-labelledby="dropdownMenu">' + getVersions(movieId) + '<a class="dropdown-item">Fix Match</a></dropdown></div><br><div class="left"><img src="'+item.poster+'" alt=""/><h3>'+item.title+'</h3><p>'+item.year+'</p></div><div class="right"><p><span>'+item.plot+'</span></p><p> Actors: <span>'+item.actors+'</span></p><p> Genre: <span>'+item.genre+'</span></p><p>File Runtime: <span>'+ item.duration +'</span>'+play+'</p><p> Movie Length: <span>'+item.runtime+'</span></p><p>Movie Quality: <span>'+item.mulitple+'</span></p><p><img class="rotten"src="/Img/rotten-tomatoes.png" alt="rotten tomaotes icon."/> <span>'+item.rottentomatoesrating+'</span></p><p><img class="rotten"src="/Img/imdb.png" alt="imdb icon."/> <span>'+item.imdbrating+'</span></p></div>');
    $('#singleMovieContainer').show();
}

function getVersions(){
    return '<a class="dropdown-item">version</a>';
}

function qualityOfMovie(movieID){
    return "<div>quality!!!!!</div>";
}

function backLink(){
    $('#singleMovieContainer').hide();
    $('#moviesDisplay').show();
    localStorage.removeItem("movie");
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
function refreshLibrary(libraryId){
    fetch('/refresh/'+ libraryId)
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

deleteLibrary = (libraryId, event) => {
    fetch('/JSON/delete/'+ libraryId)
      .then(function(res) {
        return res.json();
      }).then(library =>{
        $(event.target).parent().remove();
        fetch('/JSON/allitems')
        .then(function(res) {
            return res.json();
        }).then(data =>{
            allItems = data
            toDom(allItems);
        } 
        );
      });
}




$(document).ready(function(){
    $('#singleMovieContainer').hide();
    $('#moviesDisplay').hide();
    
    

    $('#addLibraryButton').on('click', function(){
        $('#formPopup').show();
    });
    // $('#closeForm').on('click', function(){
    //     $('#formPopup').hide();
    // });
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
      }).then(data => allItems = data).then(function(){
        if (window.performance && window.performance.navigation.type == window.performance.navigation.TYPE_BACK_FORWARD) {
            if(localStorage.getItem("movie")){
                renderMovie(localStorage.getItem("movie"));
                localStorage.removeItem("movie");
            } else {
                $('#moviesDisplay').show();
            }
        } else {
            $('#moviesDisplay').show();
        }
      });

    
    $('#search').on('keyup', function(){
        clearTimeout(timeout);
        var string = $(this).val().toLowerCase();
        timeout = setTimeout(function(){
            localStorage.removeItem("movie");
            var filtered = allItems.filter(item => ((item.title) ? item.title.toLowerCase().includes(string) : false) || ((item.genre) ? item.genre.toLowerCase().includes(string) : false));
            console.log(filtered);
            bounce(filtered);
        }, 500);
        
    });
    
}); 

$(document).mouseup(function(e) 
{

    // if the target of the click isn't the container nor a descendant of the container
    if ($("#formPopup").is(':visible'))
    {
        if (!$(".libraryForm").is(e.target) && $(".libraryForm").has(e.target).length === 0) 
        {
            $("#formPopup").hide();
        }
    }

});

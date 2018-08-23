/* ========================================================================= */
/*	Preloader
/* ========================================================================= */

jQuery(window).load(function(){

	$("#preloader").fadeOut("slow");

});

/* ========================================================================= */
/*  Welcome Section Slider
/* ========================================================================= */

$('.aaf').on("click", function () {
    
})

$('.aaf').on("click", function () {

})


$(function() {

    var Page = (function () {

        var $navArrows = $( '#nav-arrows' ),
            $nav = $( '#nav-dots > span' ),
            slitslider = $( '#slider' ).slitslider( {
                onBeforeChange : function( slide, pos ) {

                    $nav.removeClass( 'nav-dot-current' );
                    $nav.eq( pos ).addClass( 'nav-dot-current' );

                }
            } ),

            init = function() {

                initEvents();
                
            },
            initEvents = function() {

                // add navigation events
                $navArrows.children( ':last' ).on( 'click', function() {

                    slitslider.next();
                    return false;

                } );

                $navArrows.children( ':first' ).on( 'click', function() {
                    
                    slitslider.previous();
                    return false;

                } );

                $nav.each( function( i ) {
                
                    $( this ).on( 'click', function( event ) {
                        
                        var $dot = $( this );
                        
                        if( !slitslider.isActive() ) {

                            $nav.removeClass( 'nav-dot-current' );
                            $dot.addClass( 'nav-dot-current' );
                        
                        }
                        
                        slitslider.jump( i + 1 );
                        return false;
                    
                    } );
                    
                } );

            };

            return { init : init };

    })();

    Page.init();

});


function closetoogle() {
    //$(".mobilelogo").css("visibility", "hidden");
    //$(".mobileclose").css("visibility", "hidden");
    //$(".menumobilde").css("visibility", "hidden");
    

    $(".menumobilde").removeClass("mobileDown");
    $(".menumobilde").addClass("mobileUp");

    //$(".menumobilde").css("height", "0px");
}


var timermobile;
function cliktoogle() {
    //$(".menumobilde").css("visibility", "visible");
    //$(".mobilelogo").css("visibility", "visible");
    //$(".mobileclose").css("visibility", "visible");

    $(".menumobilde").css("height", $(window).height() + "px");

    $(".menumobilde").removeClass("mobileUp");
    $(".menumobilde").addClass("mobileDown");

/*
    //$("#navigation").removeClass("animated-header");
    //$("#navigation").removeClass("navigation-scroll");
    $("#navigation").removeClass("animated-header");

    $("#navigation").css("height", "85px");
    $("#navigation").css("background-color", "white");
    $(".container-fluid").css("background-color", "white");
   
    $("#logowhite").removeClass("img-logo-white");
    $("#logowhite").addClass("img-logo-blue");
    
    $(".navbar-toggle").css("border-color", "darkblue");
    
    $(".navbar-inverse .navbar-toggle").css("border-color", "darkblue");
    $(".navbar-brand a").css("color", "#fff");
    $(".navbar-inverse .navbar-nav > li > a").css("color", "rgb(15, 32, 108)");
    $(".navbar-inverse .navbar-nav > li > a").css("font-size", "18px");
    $(".navbar-inverse .navbar-nav > li > a").css("font-weight", "bold");
    $(".icon-bar").css("background-color", "#0F206C");
    
    /*
    if ($("#navbarmobile").height() > 85) {

        $(".icon-bar").css("background-color", "#fff");
        $(".navbar-inverse .navbar-nav > li > a").css("color", "#fff");
        $("#logowhite").addClass("img-logo-white");
        $(".navbar-inverse .navbar-toggle").css("border-color", "white");
        $("#navigation").css("background-color", "transparent");
        $(".container-fluid").css("background-color", "transparent");
    }
   */

   // $("#navigation").removeClass("navAniUp");
   // $("#navigation").addClass("navAniDown");
    
}

$(document).ready(function(){

    /* ========================================================================= */
	/*	Menu item highlighting
	/* ========================================================================= */

    jQuery('#nav').singlePageNav({
        offset: jQuery('#nav').outerHeight(),
        filter: ':not(.external)',
        speed: 400,
        currentClass: 'active',
        easing: 'easeInOutExpo',
        updateHash: true,
        beforeStart: function () {
            console.log('begin scrolling');
        },
        onComplete: function () {
            console.log('done scrolling');
        }
    });

    var flagDown = true;
    function AnimationListener() {
        if (flagDown) {
            $("#navigation").css("background-color", "transparent");
            $("#navigation").removeClass("navigation-scroll");
            $("#logowhite").removeClass("img-logo-blue");
            $("#navigation").removeClass("navAniUp");
            //$("#navigation").addClass('animated-header');
            //$(".navbar-brand a").css("color", "inherit");
            $(".icon-bar").css("background-color", "#fff");
            $(".navbar-inverse .navbar-nav > li > a").css("color", "#fff");
            //$("#navigation").addClass("animated-header");
            $("#logowhite").addClass("img-logo-white");
        }
        
    };

   
    //var widthlogo = $("#logowhite").css("width");
    //var heightlogo = $("#logowhite").css("height");
    var timer;
    var flagtimer = false;
    var flagenter = false;
    $("#navbarmobile").css("height", "0px");
    $(window).scroll(function () {
        if ($(window).width() > 767) {
            if ($(window).scrollTop() > 100) {
                //clearTimeout(timer);
                if (!flagtimer) {
                    flagtimer = true;
                    timer = setTimeout(function () {
                        console.log('aqui');

                        clearTimeout(timer);
                        $("#navigation").css("background-color", "white");
                        $(".container-fluid").css("background-color", "white");
                        $(".navbar-inverse .navbar-toggle").css("border-color", "darkblue");
                        $(".navbar-brand a").css("color", "#fff");
                        $(".navbar-inverse .navbar-nav > li > a").css("color", "rgb(15, 32, 108)");

                        $(".icon-bar").css("background-color", "#0F206C");
                        $("#navigation").removeClass("animated-header");
                        $("#navigation").addClass("navigation-scroll");

                        $("#logowhite").removeClass("img-logo-white");
                        $("#logowhite").addClass("img-logo-blue");

                        $("#navigation").removeClass("navAniUp");
                        $("#navigation").addClass("navAniDown");

                        $(".container-fluid").addClass("navAniDownSlider");

                        flagDown = false;

                    }, 700);
                }

                //$("#logowhite").css("visibility", "hidden");
                //$("#logowhite").css("width", "0px");
                //$("#logowhite").css("height", "0px");
                //$("#logoblue").css("visibility", "visible");
                //$("#logoblue").css("width", widthlogo);
                //$("#logoblue").css("height", heightlogo);

            } else {
                clearTimeout(timer);
                flagtimer = false;
                flagDown = true;
                $(".navbar-inverse .navbar-toggle").css("border-color", "white");
                $(".container-fluid").css("background-color", "transparent");


                $(".container-fluid").removeClass("navAniDownSlider");
               
                
                //if ($("#navigation").hasClass("navAniDown")) {
                    $("#navigation").removeClass("navAniDown");
                    $("#navigation").addClass("navAniUp");
                    var anim = document.getElementsByClassName("navAniUp");
                anim[0].addEventListener("animationend", AnimationListener, false);
                //}
              
                //$("#logoblue").css("visibility", "hidden");
                //$("#logowhite").css("width", widthlogo);
                //$("#logowhite").css("height", heightlogo);
                //$("#logowhite").css("visibility", "visible");


            }
           

        }
        else {
            if ($(window).scrollTop() > 100) {
                if (!flagenter) {
                    flagenter = true;
                    flagDown = false;

                    $("#navigation").css("background-color", "white");
                    $(".container-fluid").css("background-color", "white");
                    $(".navbar-inverse .navbar-toggle").css("border-color", "darkblue");
                    $(".navbar-brand a").css("color", "#fff");
                    $(".navbar-inverse .navbar-nav > li > a").css("color", "rgb(15, 32, 108)");

                    $(".icon-bar").css("background-color", "#0F206C");
                    $("#navigation").removeClass("animated-header");
                    $("#navigation").addClass("navigation-scroll");

                    $("#logowhite").removeClass("img-logo-white");
                    $("#logowhite").addClass("img-logo-blue");

                    $("#navigation").removeClass("navAniUp");
                    $("#navigation").addClass("navAniDown");

                    $(".container-fluid").addClass("navAniDownSlider");

                    /*
                    //$(".container-fluid").css("background-color", "white");

                    $("#navigation").removeClass("navbar-inverse");
                    $("#navigation").css("background-color", "white");

                    $(".navbar-toggle").css("border-color", "darkblue");
                    $(".navbar-inverse .navbar-toggle").css("border-color", "darkblue");
                    $(".navbar-brand a").css("color", "#fff");
                    $(".navbar-inverse .navbar-nav > li > a").css("color", "rgb(15, 32, 108)");
                    $(".nav > li > a").css("font-size", "13px");
                    //$(".navbar-brand a").css("color", "#fff");
                   // $(".nav > li > a").css("color", "rgb(15, 32, 108)");
                    //$(".nav > li > a").css("font-weight", "bold");

                  
                    $(".icon-bar").css("background-color", "#0F206C");

                    //$("#navigation").removeClass("animated-header");
                    //$("#navigation").addClass("navigation-scroll");

                    $("#logowhite").removeClass("img-logo-white");
                    $("#logowhite").addClass("img-logo-blue");
                    $("#navigation").css("height", "85px");*/
                }


            }
            else {
                if ($("#navbarmobile").height() <= 1) {
                    flagenter = false;
                    flagDown = true;

                    $(".navbar-inverse .navbar-toggle").css("border-color", "white");
                    $(".container-fluid").css("background-color", "transparent");

                    $(".container-fluid").removeClass("navAniDownSlider");

                    $("#navigation").removeClass("navAniDown");
                    $("#navigation").addClass("navAniUp");
                    var anim = document.getElementsByClassName("navAniUp");
                    anim[0].addEventListener("animationend", AnimationListener, false);


                    /*
                    $("#navigation").css("background-color", "transparent");
                    $(".container-fluid").css("background-color", "transparent");

                    $("#navigation").removeClass("navigation-scroll");
                    $("#logowhite").removeClass("img-logo-blue");
                    $(".icon-bar").css("background-color", "#fff");
                    $(".navbar-toggle").css("border-color", "white");
                    $("#logowhite").addClass("img-logo-white");*/

                }
            }
        }
       
    });

    
	
	/* ========================================================================= */
	/*	Fix Slider Height
	/* ========================================================================= */	

    // Slider Height
    var slideHeight = $(window).height();
    
    $('#home-slider, #slider, .sl-slider, .sl-content-wrapper').css('height',slideHeight);

    $(window).resize(function () {
        'use strict',
        slideHeight = $(window).height();
        $('#home-slider, #slider, .sl-slider, .sl-content-wrapper').css('height', slideHeight);
        if ($(window).width() > 812) {

        } 

    });
	
	$("#works, #testimonial").owlCarousel({	 
		navigation : true,
		pagination : false,
		slideSpeed : 700,
		paginationSpeed : 400,
		singleItem:true,
		navigationText: ["<i class='fa fa-angle-left fa-lg'></i>","<i class='fa fa-angle-right fa-lg'></i>"]
	});
	
	
	/* ========================================================================= */
	/*	Featured Project Lightbox
	/* ========================================================================= */

    $(".fancybox")
        .fancybox({
		padding: 0,

		closeClick : true,
		
		openEffect  : 'none',
		closeEffect : 'none',
		nextEffect  : 'none',
		prevEffect  : 'none',
		padding     : 0,
            margin: [20, 60, 20, 60],


            beforeShow: function () {

                var id = $(this.element)[0].id;
                 
                this.title = $(this.element).attr('title');
            this.title = '<h3>' + this.title + '</h3>' + '<p>' + $(this.element).parents('.portfolio-item').find('img').attr('alt') + '</p>';
            
            $.ajax({
                type: 'GET',
                url: 'https://ofj7hyky9i.execute-api.us-east-2.amazonaws.com/prod/dunritegetimage',
                dataType: 'json',
                data: {},
                success: function (data) {
                    var body = JSON.parse(data["body"]);
                    var items = body['Contents'];
                    var imgs = [];
                    for (var x = 0; x < items.length; x++) {
                        var key = items[x]["Key"];
                        var temp = key.split('/');
                        if (temp.length > 1 && temp[1] !== "") {
                            if (temp[0] === "Pets" && id === "Pets" ) {
                                imgs.push(
                                    {
                                        href: 'https://s3.us-east-2.amazonaws.com/dunrite-website/' + key
                                    }
                                );
                            }
                            else if (temp[0] === "Outdoor" && id === "Outdoor") {
                                imgs.push(
                                    {
                                        href: 'https://s3.us-east-2.amazonaws.com/dunrite-website/' + key
                                    }
                                );
                            }
                            else if (temp[0] === "Passion" && id === "Passion") {
                                imgs.push(
                                    {
                                        href: 'https://s3.us-east-2.amazonaws.com/dunrite-website/' + key
                                    }
                                );
                            }

                        }

                    }


                    if (imgs.length > 0) {
                        $.fancybox.open(imgs,
                            {
                                padding: 0,
                                //aspectRatio: false,
                                //width: '50%',
                                //height: '50%',
                              
                         
                            }
                        );
                    }
                    
            


                    //$.fancybox.addContent(temp1);

                    //ajax response example for left (previous) image:
                    /*
                    var response = [{
                        src: '/images/photo/0d/10/0d108198f2374bed23387f07a7cbecd5.jpg',
                    }];

                    instance.createGroup(response);

                    var obj = instance.group.pop();

                    instance.group.unshift(obj);

                    instance.jumpTo(1)*/

                }
            });


		},
		
		helpers: {
		    title: null,
            /*
			title : { 
				type: 'inside' 
			},*/
			overlay : {
				css : {
					'background' : 'rgba(0,0,0,0.8)'
				}
			}
		},
        
            afterLoad: function () {
                $.extend(this, {
                    //aspectRatio: false,
                    //width: '50%',
                    //height: '50%',
                });
            }
    });

    //Set the carousel options
    $('#quote-carousel').carousel({
        pause: true,
        interval: 4000,
    });
	
});



$(function () {
    $('#frmContact').on('submit', function (e) {
        e.preventDefault();
        var subject = $('#subject').val(),
            email = $('#email').val(),
            message = $('#message').val();

        var data = { subject: subject, email: email, message: message };

        $.ajax({
            type: 'POST',
            url: '/Home/SendEmail',
            data: data,
            success: function (res) {
                $('#frmContact').hide();
                swal("Thanks!", "Your email was successfully sent!", "success")
                    .then((value) => {
                        $('.responseText').append("Thanks for your email.");
                    });
            },
            error: function (res) {
                swal("Oops!", "Something wrong happened when trying to send your email!", "error");
            }
        });
    });
});

var wow = new WOW ({
	offset:       75,          // distance to the element when triggering the animation (default is 0)
	mobile:       false,       // trigger animations on mobile devices (default is true)
});
wow.init();

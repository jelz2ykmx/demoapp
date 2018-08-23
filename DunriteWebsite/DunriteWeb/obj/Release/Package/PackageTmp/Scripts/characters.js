$(window).resize(function () {
});

function rizediv() {
    for (var x = 1; x <= 17; x++) {
        $('#' + (x).toString()).css("margin-left", "5px");
    }
    //var count1 = Math.floor($('.centered-div').width() / 110);
    //var count2 = Math.floor(($('.centered-div').width() - 40) / 110);
    var count1 = Math.floor(994 / 110);
    var count2 = Math.floor((994 - 40) / 110);
    var start = count1;
    var flag = true;
    var impar = true;
    while (flag) {
        if (impar) {
            $('#' + (start + 1).toString()).css("margin-left", "60px");
            impar = false;
            start = start + count2;
        }
        else {
            impar = true;
            start = start + count1;
        }


        if (start > 16) {
            flag = false;
        }
    }
    

};

function hiddediv() {
    for (var x = 2; x <= 17; x++) {
        $('#pi' + x.toString()).css("display", "none");

    };
};

var timer;
var timer2;
var lastid = 1;
var start = true;
var time1 = 200;
var time2 = 500;

function showdiv(id) {
    clearTimeout(timer);
    hiddediv();
    if (id !== 1) {
       $('#1').css("background-image", "url('/Content/img/about_us/Group 16.png')");
    }
    if (id == 1) {
        $('#1').css("background-image", "url('/Content/img/about_us/mouseover/Group 16.png')");
    }
    else if (id === "0" && lastid !== 0) {
       
        timer2 = setTimeout(function () {
            $('#1').css("background-image", "url('/Content/img/about_us/mouseover/Group 16.png')");

            //$('#pi' + lastid).removeClass("charactertextoHidden");

            $('#pi1').css("display", "block");
            $('#pi1').addClass("charactertexto");
        }, time1);

    }
    else {
        clearTimeout(timer2);
        $('#pi1').css("display", "none");
    }
   
    if (!start || id !== 1) {
        timer = setTimeout(function () {
            lastid = id;
            $('#pi' + lastid).removeClass("charactertextoHidden");
            $('#pi' + id).css("display", "block");
            $('#pi' + id).addClass("charactertexto");

        }, time2);
    }
    else {
        start = false;
    }

    
  
    //

};

function showdivHide() {
    if (lastid != 0) {
        $('#pi' + lastid).addClass("charactertextoHidden");
    }
};

$(function () {

    var Page = (function () {

        if ($(window).width() <= 1024) {
            time1 = 0;
            time2 = 0;
        }

        rizediv()
        hiddediv()

        $('#1').css("background-image", "url('/Content/img/about_us/mouseover/Group 16.png')");
        $('#pi1').css("display", "block");
        $('#pi1').addClass("charactertexto");
        lastid = 1;
       
        $("#1").mouseover(function () {
            showdiv("1");
        })
        .mouseout(function () {
            showdivHide();
            showdiv("0");
        })


        $("#2").mouseover(function () {
            showdiv("2");
        })
         .mouseout(function () {
             showdivHide();
             showdiv("0");
         })

        $("#3").mouseover(function () {
            showdiv("3");
        })
         .mouseout(function () {
             showdivHide();
             showdiv("0");
         })

        $("#4").mouseover(function () {
            showdiv("4");
        })
            .mouseout(function () {
             showdivHide();
             showdiv("0");
         })

        $("#5").mouseover(function () {
            showdiv("5");
        })
         .mouseout(function () {
             showdivHide();
             showdiv("0");
         })

        $("#6").mouseover(function () {
            showdiv("6");
        })
         .mouseout(function () {
             showdivHide();
             showdiv("0");
         })

        $("#7").mouseover(function () {
            showdiv("7");
        })
         .mouseout(function () {
             showdivHide();
             showdiv("0");
         })

        $("#8").mouseover(function () {
            showdiv("8");
        })
         .mouseout(function () {
             showdivHide();
             showdiv("0");
         })

        $("#9").mouseover(function () {
            showdiv("9");
        })
         .mouseout(function () {
             showdivHide();
             showdiv("0");
         })

        $("#10").mouseover(function () {
            showdiv("10");
        })
         .mouseout(function () {
             showdivHide();
             showdiv("0");
         })

        $("#11").mouseover(function () {
            showdiv("11");
        })
         .mouseout(function () {
             showdivHide();
             showdiv("0");
         })

        $("#12").mouseover(function () {
            showdiv("12");
        })
         .mouseout(function () {
             showdivHide();
             showdiv("0");
         })

        $("#13").mouseover(function () {
            showdiv("13");
        })
         .mouseout(function () {
             showdivHide();
             showdiv("0");
         })

        $("#14").mouseover(function () {
            showdiv("14");
        })
         .mouseout(function () {
             showdivHide();
             showdiv("0");
         })

        $("#15").mouseover(function () {
            showdiv("15");
        })
         .mouseout(function () {
             showdivHide();
             showdiv("0");
         })

        $("#16").mouseover(function () {
            showdiv("16");
        })
        .mouseout(function () {
            showdivHide();
             showdiv("0");
        })
        $("#17").mouseover(function () {
            showdiv("17");
        })
         .mouseout(function () {
             showdivHide();
             showdiv("0");
            })

        //touchevents
        
        $("#1").on('touchstart', function () {
            showdiv("1");
        });

        $("#2").on('touchstart', function () {
            showdiv("2");
        });

        $("#3").on('touchstart', function () {
            showdiv("3");
        });

        $("#4").on('touchstart', function () {
            showdiv("4");
        });

        $("#5").on('touchstart', function () {
            showdiv("5");
        });

        $("#6").on('touchstart', function () {
            showdiv("6");
        });

        $("#7").on('touchstart', function () {
            showdiv("7");
        });

        $("#8").on('touchstart', function () {
            showdiv("8");
        });

        $("#9").on('touchstart', function () {
            showdiv("9");
        });

        $("#10").on('touchstart', function () {
            showdiv("10");
        });

        $("#11").on('touchstart', function () {
            showdiv("11");
        });

        $("#12").on('touchstart', function () {
            showdiv("12");
        });

        $("#13").on('touchstart', function () {
            showdiv("13");
        });

        $("#14").on('touchstart', function () {
            showdiv("14");
        });

        $("#15").on('touchstart', function () {
            showdiv("15");
        });

        $("#16").on('touchstart', function () {
            showdiv("16");
        });

        $("#17").on('touchstart', function () {
            showdiv("17");
        });
    })();

   

});


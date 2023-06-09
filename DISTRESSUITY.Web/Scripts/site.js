    $(function () {
        // Remove Search if user Resets Form or hits Escape!
		$('body, .navbar-collapse form[role="search"] button[type="reset"]').on('click keyup', function(event) {
			console.log(event.currentTarget);
			if (event.which == 27 && $('.navbar-collapse form[role="search"]').hasClass('active') ||
				$(event.currentTarget).attr('type') == 'reset') {
				closeSearch();
			}
		});

		function closeSearch() {
            var $form = $('.navbar-collapse form[role="search"].active')
    		$form.find('input').val('');
			$form.removeClass('active');
		}

		// Show Search if form is not active // event.preventDefault() is important, this prevents the form from submitting
		$(document).on('click', '.navbar-collapse form[role="search"]:not(.active) button[type="submit"]', function(event) {
			event.preventDefault();
			var $form = $(this).closest('form'),
				$input = $form.find('input');
			$form.addClass('active');
			$input.focus();
			
			
			

		});
		// ONLY FOR DEMO // Please use $('form').submit(function(event)) to track from submission
		// if your form is ajax remember to call `closeSearch()` to close the search container
		$(document).on('click', '.navbar-collapse form[role="search"].active button[type="submit"]', function(event) {
			event.preventDefault();
			var $form = $(this).closest('form'),
				$input = $form.find('input');
			$('#showSearchTerm').text($input.val());
            closeSearch()
		});
    });
	
 $(document).ready(function () {
	 
	$('.add-positions-button .btn').click(function(event) {
		
      $(".add-position ").removeClass("display-none");
    });
	$('.add-positions-button .btn').click(function(e){e.preventDefault()});  
    $('.forgot-pass').click(function(event) {
      $(".pr-wrap").toggleClass("show-pass-reset");
    }); 
    
    $('.pass-reset-submit').click(function(event) {
      $(".pr-wrap").removeClass("show-pass-reset");
    }); 
 });

//$(function() {

//    $('#login-form-link').click(function(e) {
//		$("#login-form").delay(100).fadeIn(100);
// 		$("#register-form").fadeOut(100);
//		$('#register-form-link').removeClass('active');
//		$(this).addClass('active');
//		e.preventDefault();
//	});
//	$('#register-form-link').click(function(e) {
//		$("#register-form").delay(100).fadeIn(100);
// 		$("#login-form").fadeOut(100);
//		$('#login-form-link').removeClass('active');
//		$('#login-form-link').addClass('display-none');
//		$(this).addClass('active');
//		e.preventDefault();
//	});
//	$('#register-form-link').click(function(e) {
//		$('#login-form-link').removeClass('display-block');
//		$('.signup').addClass('display-block');
//		$('.signup').addClass('active');
		
//	});
//	$('#login-form-link-bottom').click(function(e) {
//		$('.signup').removeClass('display-block');
//		$('.signup').addClass('display-none');
//		$('#login-form-link').addClass('active');
//		$('#login-form-link').addClass('display-block');
//		$("#login-form").delay(100).fadeIn(100);
// 		$("#register-form").fadeOut(100);
//		$('#register-form-link').removeClass('active');
		
//	});
//	$('#register-submit').click(function(e){e.preventDefault()});

//}

//image-uploader

//$(document).on('click', '#close-preview', function()
//{ 
//    $('.image-preview').popover('hide');
//    // Hover befor close the preview
//    $('.image-preview').hover(
//        function () {
//           $('.image-preview').popover('show');
//        }, 
//         function () {
//           $('.image-preview').popover('hide');
//        }
//    );    
//});

//$(function() {
//    // Create the close button
//    var closebtn = $('<button/>', {
//        type:"button",
//        text: 'x',
//        id: 'close-preview',
//        style: 'font-size: initial;',
//    });
//    closebtn.attr("class","close pull-right");
//    // Set the popover default content
//    $('.image-preview').popover({
//        trigger:'manual',
//        html:true,
//        title: "<strong>Preview</strong>"+$(closebtn)[0].outerHTML,
//        content: "There's no image",
//        placement:'bottom'
//    });
//    // Clear event
//    $('.image-preview-clear').click(function(){
//        $('.image-preview').attr("data-content","").popover('hide');
//        $('.image-preview-filename').val("");
//        $('.image-preview-clear').hide();
//        $('.image-preview-input input:file').val("");
//        $(".image-preview-input-title").text("Browse"); 
//    }); 
//    // Create the preview image
//    $(".image-preview-input input:file").change(function (){     
//        var img = $('<img/>', {
//            id: 'dynamic',
//            width:250,
//            height:200
//        });      
//        var file = this.files[0];
//        var reader = new FileReader();
//        // Set preview image into the popover data-content
//        reader.onload = function (e) {
//            $(".image-preview-input-title").text("Change");
//            $(".image-preview-clear").show();
//            $(".image-preview-filename").val(file.name);            
//            img.attr('src', e.target.result);
//            $(".image-preview").attr("data-content",$(img)[0].outerHTML).popover("show");
//        }        
//        reader.readAsDataURL(file);
//    });  
//});

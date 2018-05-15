$(document).ready(function(){toggle_nav_container();gotoByScroll();});var toggle_nav_container=function(){var $toggleButton=$('#toggle_m_nav');$navContainer=$('#m_nav_container');$menuButton=$('#m_nav_menu')
$menuButtonBars=$('.m_nav_ham');$wrapper=$('#wrapper');$toggleButton.on("click",function(){var $viewportWidth=$(window).width();if($navContainer.is(':hidden')){$wrapper.removeClass('closed_wrapper');$wrapper.addClass("open_wrapper");$navContainer.slideDown(200).addClass('container_open').css("z-index","2");$menuButtonBars.removeClass('button_closed');$menuButtonBars.addClass('button_open');$("#m_ham_1").addClass("m_nav_ham_1_open");$("#m_ham_2").addClass("m_nav_ham_2_open");$("#m_ham_3").addClass("m_nav_ham_3_open");}else{$navContainer.css("z-index","0").removeClass('container_open').slideUp(200)
$menuButtonBars.removeClass('button_open')
$menuButtonBars.addClass('button_closed')
$wrapper.removeClass('open_wrapper')
$wrapper.addClass("closed_wrapper")
$("#m_ham_1").removeClass("m_nav_ham_1_open");$("#m_ham_2").removeClass("m_nav_ham_2_open");$("#m_ham_3").removeClass("m_nav_ham_3_open");}});}
var gotoByScroll=function(){$(".m_nav_item a").on("click",function(e){$('html,body').animate({scrollTop:$($(this).attr("href")).offset().top-50},"slow");});}
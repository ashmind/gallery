$(function() {
    function fancybox() {
        $(".wall a").fancybox({
            transitionIn	: 'fade',
            transitionOut	: 'fade',
            overlayOpacity  : 0.8,
            overlayColor    : '#000'
        });        
    }

    $("#left a.album-name").click(function(event) {
        event.preventDefault();

        var that = $(this);
        $.get(that.attr('href'), {}, function(html) {
            $('#main').html(html);

            that.parent().find('.selected').removeClass('selected');
            that.addClass('selected');

            fancybox();
        });
    });

    $("#left a.more").click(function(event) {
        event.preventDefault();

        var that = $(this);
        $.get(that.attr('data-ajax'), {}, function(html) {
            that.replaceWith(html);
        });
    });
});
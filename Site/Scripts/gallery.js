﻿$(function() {
    $.history.init(function(hash) {
        if ($.history.noaction)
            return;

        var parts = hash.replace(/^\\/, '').split('\\');
        var albumLink = $("#left a.album-name[data-id='" + parts[0] + "']");

        if (albumLink.length == 0)
            return;

        var selectItem = function() {
            if (parts.length == 1)
                return;

            $.history.noaction = true;
            $(".wall a[data-name='" + parts[1] + "']")
                .click();
            $.history.noaction = false;
        };

        if (!albumLink.hasClass('selected')) {
            loadAlbum(albumLink, selectItem);
        }
        else {
            selectItem();
        }
    });
    setupAlbum();

    $("#left a.album-name").live('click', function(event) {
        event.preventDefault();
        $.history.load("\\" + $(this).attr('data-id'));
    });

    $("#left a.more").live('click', function(event) {
        event.preventDefault();

        var a = $(this);
        $.get(a.attr('data-ajax'), {}, function(html) {
            a.replaceWith(html);
        });
    });    
});

function loadAlbum(a, onsuccess) {
    $.get(a.attr('href'), {}, function(html) {
        $('#main').html(html);

        a.parent().find('.selected').removeClass('selected');
        a.addClass('selected');

        setupAlbum();

        document.title = document.title.replace(/:.+$/, "") + ": " + a.text();

        if (onsuccess)
            onsuccess();
    });        
}

function setupAlbum() {
    $(".wall a").fancybox({
        padding	        : 0,
        transitionIn	: 'fade',
        transitionOut	: 'fade',
        overlayOpacity  : 0.8,
        overlayColor    : '#000',
        titlePosition   : 'over',
        titleFormat     : function(title, currentArray, currentIndex) {
            var a = currentArray.eq(currentIndex);
            return "<span id='fancybox-title-over' class='image-actions'>" +
                "<a href='" + a.attr('data-action-download') + "'>Download</a>" +
                "<a href='" + a.attr('data-action-comment') + "'>Comment</a>"
            "</span>";
        },
        onComplete      : function(currentArray, currentIndex) {
            var a = currentArray.eq(currentIndex);
            a.parent().toggleClass('selected');
            if (!$.history.noaction) {
                $.history.noaction = true;
                $.history.load("\\" + a.parents('.album-view').attr('data-id') + '\\' + a.attr('data-name'));
                $.history.noaction = false;
            }
        },
        onClosed        : function(currentArray, currentIndex) {
            var a = currentArray.eq(currentIndex);
            $.history.noaction = true;
            $.history.load("\\" + a.parents('.album-view').attr('data-id'));
            $.history.noaction = false;
        }
    });

    $("#main").scrollTop(0);
    var imgs = $(".wall img");
    imgs.lazyload({
        placeholder : imgs.eq(0).attr('src'),
        container   : $("#main"),
        effect      : "fadeIn"
    });

    setupSecurityPanel();

    /*$('.wall').dragToSelect({
        selectables  : '.item',
        selectOnMove : true/ *,
        onHide       : function () {
            var selected = $('.wall .item.selected');
            if (selected.length > 0) {
                $("#main").css('right', '12.6em');
                $("#right-menu").show();
            }
            else {
                $("#right-menu").hide();
                $("#main").css('right', '0');
            }
        }* /
    });*/
}

function setupSecurityPanel() {
    $("a.album-visible-to").fancybox({
        transitionIn	: 'fade',
        transitionOut	: 'fade',
        overlayOpacity  : 0.8,
        overlayColor    : '#000',
        onComplete      : function() {
            $('form.grant img')
                .click(function() {
                    var group = $(this).parent();

                    group.toggleClass('granted');
                    var hidden = group.find('input');
                    hidden.val(hidden.val() == "" ? group.attr('data-key') : "");
                });

            $('form.grant').ajaxForm({
                success : function() {
                    $.fancybox.close();
                    loadAlbum($("#left a.album-name.selected")); // reloads album
                }
            });
        }
    });
}
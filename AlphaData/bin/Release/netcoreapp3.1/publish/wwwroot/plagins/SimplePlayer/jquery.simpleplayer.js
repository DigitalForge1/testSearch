/*
 * SimplePlayer - A jQuery Plugin
 * @requires jQuery v1.4.2 or later
 *
 * SimplePlayer is a html5 audio player
 *
 * Licensed under the MIT:
 * http://www.opensource.org/licenses/mit-license.php
 *
 * Copyright (c) 2010-, Yuanhao Li (jay_21cn -[at]- hotmail [*dot*] com)
 */
(function($) {
    $.fn.player = function(settings) {
        var config = {
            conteinerWidth: '200px',
            progressbarWidth: '200px',
            progressbarHeight: '5px',
            progressbarColor: '#22ccff',
            progressbarBGColor: '#eeeeee',
            defaultVolume: 0.8
        };

        if (settings) {
            $.extend(config, settings);
        }

        var playControl = '<span class="simpleplayer-play-control"></span>';
        var stopControl = '<span class="simpleplayer-stop-control"></span>';
        
        function formatTime(sec) {
			min = Math.floor(sec / 60);
			min = (min >= 10) ? min : "0" + min;
			sec = Math.floor(sec % 60);
			sec = (sec >= 10) ? sec : "0" + sec;
			return min + ":" + sec;
		}
        
        this.each(function() {
            $(this).before('<div class="simple-player-container" style="width: ' + config.conteinerWidth +';">');
            $(this).after('</div>');
            $(this).parent().find('.simple-player-container').prepend(
                '<div><ul>' + 
                    '<li style="display: inline-block; padding: 0 5px; "><a style="text-decoration: none;"' +
                    ' class="start-button" href="javascript:void(0)">' + playControl + '</a></li>' + 
                    '<li class="progressbar-wrapper" style="display: inline-block; cursor: pointer; width:' + config.progressbarWidth + ';">' + 
                        '<span style="display: block; background-color: ' + config.progressbarBGColor + '; width: 100%; ">' + 
                        '<span class="progressbar" style="display: block; background-color: ' + config.progressbarColor +
                                                         '; height: ' + config.progressbarHeight + '; width: 0%; ">' +
                        '</span></span>' + 
                    '</li>' + 
                '<span class="time-current"></span>' +
                '</ul></div>'
            );

            var simplePlayer = $(this).get(0);
            var button = $(this).parent().find('.start-button');
            var progressbarWrapper = $(this).parent().find('.progressbar-wrapper');
            var progressbar = $(this).parent().find('.progressbar');
            var timeCurrent = $(this).parent().find('.time-current');
            timeCurrent.text('00:00');
            simplePlayer.volume = config.defaultVolume;

            button.click(function() {
                if (simplePlayer.paused) {
                    /*stop all playing songs*/
                    $.each($('audio'), function () {
    					this.pause();
						$(this).parent().find('.simpleplayer-stop-control').addClass('simpleplayer-play-control').removeClass('simpleplayer-stop-control');
					});
                    simplePlayer.play();
                    $(this).find('.simpleplayer-play-control').addClass('simpleplayer-stop-control').removeClass('simpleplayer-play-control');
                } else {
                    simplePlayer.pause();
                    $(this).find('.simpleplayer-stop-control').addClass('simpleplayer-play-control').removeClass('simpleplayer-stop-control');
                }
            });

            progressbarWrapper.click(function(e) {
                if (simplePlayer.duration != 0) {
                    left = $(this).offset().left;
                    offset = e.pageX - left;
                    percent = offset / progressbarWrapper.width();
                    duration_seek = percent * simplePlayer.duration;
                    simplePlayer.currentTime = duration_seek;
                }
            });


            $(simplePlayer).bind('ended', function(evt) {
                simplePlayer.pause();
                button.find('.simpleplayer-stop-control').addClass('simpleplayer-play-control').removeClass('simpleplayer-stop-control');
                progressbar.css('width', '0%');
                timeCurrent.text('00:00');
            });
            
            

            $(simplePlayer).bind('timeupdate', function(e) {
                duration = this.duration;
                time = this.currentTime;
                fraction = time / duration;
                percent = fraction * 100;
                
                if (percent) {progressbar.css('width', percent + '%');
                              timeCurrent.text(formatTime(time));
                             };
            });

            if (simplePlayer.duration > 0) {
                $(this).parent().css('display', 'inline-block');
            }

            if ($(this).attr('autoplay') == 'autoplay') {
                button.click();
            }
        });

        return this;
    };
})(jQuery);

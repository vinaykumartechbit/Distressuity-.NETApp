app.directive('bsTooltip', function () {
    return {
        restrict: 'A',
        link: function (scope, element, attrs) {
            $(element).hover(function () {
                // on mouseenter
                var projectTitle = $(element).find('.projectTitle').text();
                var userName = $(element).find('.userName').text();
                //"<h1><strong>userName</strong> <em>" + projectTitle + "</em></h1>"
                //"<div class='li-tooltip-container'> <p><strong>" + userName + "</strong></p> <p>{{conversation.ProjectModel.Title}}</p>  </div>"
                $(element).tooltip({
                    title: "<p><strong>" + userName + "</strong></p> <p>" + projectTitle + "</p>",
                    template: '<div class="tooltip" role="tooltip"><div class="tooltip-arrow"></div><div class="tooltip-inner"></div></div>'
                    , html: true, placement: 'top'
                });
            }, function () {
                // on mouseleave
                $(element).tooltip('hide');
            });
        }
    };
});
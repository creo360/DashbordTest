var app = angular.module('app', ['ui.grid', 'ui.grid.pagination', 'ngAnimate', 'ui.grid.resizeColumns'])

$(document).ready(function () {


    angular.element(document).ready(function () {
        angular.bootstrap('#gridContainer', ['app']);

    });



    $('.api-link').click(function (e) {
        e.preventDefault();
        var url = $(this).attr('href');


        $.ajax({

            url: url,
            type: 'POST',
            dataType: 'html',
            success: function (data) {
                $('#pageContent').empty();
                $('#pageContent').append(data);

                angular.element(document).ready(function () {
                    angular.bootstrap('#gridContainer', ['app']);

                });

            }

        });



    });

});

/*

$.getScript("/Scripts/ui-grid-unstable.js").done(function () {
    $.getScript("/Scripts/app/app.js").done(function () {
        $.getScript("/Scripts/app/controllers/AllocInstController.js").done(function () {
            alert('Angular loaded');
        });
    });
});

*/




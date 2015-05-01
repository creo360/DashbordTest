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
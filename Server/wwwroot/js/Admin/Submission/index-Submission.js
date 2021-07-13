var indexSubmission = function () {

    this.initialize = function () {
        registerEvents();

    };
    function registerEvents() {
        $("#goToAssignment").click(function () {
            window.location.href = window.location.href + '/assignment';
        });
    }
};


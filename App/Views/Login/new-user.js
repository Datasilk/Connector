﻿S.login = {
    watchPass: function (e) {

    },

    createAccount: function () {
        //save new password for user
        var name = $('#name').val();
        var email = $('#email').val();
        var username = $('#username').val();
        var pass = $('#password').val();
        var pass2 = $('#password2').val();
        var msg = $('.login .message');

        //validate name
        if (name == '') {
            S.message.show(msg, 'error', 'You must provide your name');
            return;
        }

        if (email != '') {
            //validate email
            if (!S.login.validateEmail(email)) {
                S.message.show(msg, 'error', 'The provided email address is not valid');
                return;
            }
        }

        //validate username
        if (username == '') {
            S.message.show(msg, 'error', 'You must provide a valid user name');
            return;
        }

        //validate password
        if (pass == '' || pass2 == '') {
            S.message.show(msg, 'error', 'You must type in your password twice');
            return;
        }
        if (pass != pass2) {
            S.message.show(msg, 'error', 'Your passwords do not match');
            return;
        }
        if (pass.length < 8) {
            S.message.show(msg, 'error', 'Your password must be at least 8 characters long');
            return;
        }

        //disable button
        $('#btnsavepass').prop("disabled", "disabled");

        //send new account info to server
        S.ajax.post('User/CreateUserAccount', { name: name, email: email, username: username, password: pass }, function (data) {
            //callback, replace form with message
            if (data == 'success') {
                //show success message
                window.location.reload();
            } else {
                //show error message
                S.message.show(msg, 'error', 'An error occurred while trying to create your account');
            }
        }, function () {
            S.message.show(msg, 'error', 'An error occurred while trying to create your account');
        });
    },

    validateEmail: function (email) {
        return /^([^\x00-\x20\x22\x28\x29\x2c\x2e\x3a-\x3c\x3e\x40\x5b-\x5d\x7f-\xff]+|\x22([^\x0d\x22\x5c\x80-\xff]|\x5c[\x00-\x7f])*\x22)(\x2e([^\x00-\x20\x22\x28\x29\x2c\x2e\x3a-\x3c\x3e\x40\x5b-\x5d\x7f-\xff]+|\x22([^\x0d\x22\x5c\x80-\xff]|\x5c[\x00-\x7f])*\x22))*\x40([^\x00-\x20\x22\x28\x29\x2c\x2e\x3a-\x3c\x3e\x40\x5b-\x5d\x7f-\xff]+|\x5b([^\x0d\x5b-\x5d\x80-\xff]|\x5c[\x00-\x7f])*\x5d)(\x2e([^\x00-\x20\x22\x28\x29\x2c\x2e\x3a-\x3c\x3e\x40\x5b-\x5d\x7f-\xff]+|\x5b([^\x0d\x5b-\x5d\x80-\xff]|\x5c[\x00-\x7f])*\x5d))*$/.test(email);
    }
}

//add event listeners
$('#password, #password2').on('input', S.login.watchPass);
$('.login form').on('submit', function (e) {
    e.preventDefault();
    e.cancelBubble = true;
    S.login.createAccount();
    return false;
});
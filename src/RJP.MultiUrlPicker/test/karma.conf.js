// Karma configuration
// Generated on Tue Jun 03 2014 08:15:43 GMT+0200 (CEST)

module.exports = function (config) {

    config.set({

        // base path that will be used to resolve all patterns (eg. files, exclude)
        basePath: '..',


        // frameworks to use
        // available frameworks: https://npmjs.org/browse/keyword/karma-adapter
        frameworks: ['jasmine'],


        // list of files / patterns to load in the browser
        files: [
          'test/assets/lib/jquery/jquery-2.0.3.min.js',
          'test/assets/lib/angular/1.1.5/angular.js',
          'test/assets/lib/angular/1.1.5/angular-cookies.min.js',
          'test/assets/lib/angular/1.1.5/angular-mocks.js',
          'test/assets/lib/angular/angular-ui-sortable.js',

          'test/assets/lib/underscore/underscore.js',
          'test/assets/lib/umbraco/Extensions.js',
          'test/assets/lib/lazyload/lazyload.min.js',

          'test/app.conf.js',
          'test/assets/js/umbraco.*.js',

          'app/scripts/controllers/*.js',
          'test/**/*.spec.js'
        ],


        // list of files to exclude
        exclude: [
          'test/assets/js/umbraco.httpbackend.js'
        ],


        // preprocess matching files before serving them to the browser
        // available preprocessors: https://npmjs.org/browse/keyword/karma-preprocessor
        preprocessors: {

        },


        // test results reporter to use
        // possible values: 'dots', 'progress'
        // available reporters: https://npmjs.org/browse/keyword/karma-reporter
        reporters: ['progress', 'xml'],


        // web server port
        port: 9876,


        // enable / disable colors in the output (reporters and logs)
        colors: true,


        // level of logging
        // possible values: config.LOG_DISABLE || config.LOG_ERROR || config.LOG_WARN || config.LOG_INFO || config.LOG_DEBUG
        logLevel: config.LOG_INFO,


        // enable / disable watching file and executing tests whenever any file changes
        autoWatch: false,


        // start these browsers
        // available browser launchers: https://npmjs.org/browse/keyword/karma-launcher
        browsers: ['PhantomJS'],


        // Continuous Integration mode
        // if true, Karma captures browsers, runs the tests and exits
        singleRun: true
    });
};
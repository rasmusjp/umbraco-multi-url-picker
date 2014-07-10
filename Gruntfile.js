module.exports = function (grunt) {
    require('load-grunt-tasks')(grunt);
    require('time-grunt')(grunt);
    require('grunt-karma')(grunt);

    //cant load this with require
    grunt.loadNpmTasks('grunt-contrib-jshint');

    if (grunt.option('target') && !grunt.file.isDir(grunt.option('target'))) {
        grunt.fail.warn('The --target option specified is not a valid directory');
    }

    grunt.initConfig({
        pkg: grunt.file.readJSON('package.json'),
        dest: grunt.option('target') || 'dist',
        basePath: 'src/RJP.MultiUrlPicker/App_Plugins/RJP.MultiUrlPicker',

        watch: {
            options: {
                atBegin: true
            },

            testControllers: {
                files: [
                    '<%= basePath %>/**/*.js',
                    'src/RJP.MultiUrlPicker/test/specs/**/*.spec.js'
                ],
                tasks: ['jshint', 'test']
            },
        },

        copy: {
            nuget: {
                expand: true,
                cwd: '<%= basePath %>',
                src: '**',
                dest: 'tmp/nuget/content/'
            },

            umbraco: {
                expand: true,
                cwd: '<%= basePath %>/',
                src: '**',
                dest: 'tmp/umbraco/'
            },

            testAssets: {
                expand: true,
                cwd: '<%= dest %>',
                src: ['js/umbraco.*.js', 'lib/**/*.js'],
                dest: 'src/RJP.MultiUrlPicker/test/assets/'
            }
        },

        template: {
            nuspec: {
                options: {
                    data: {
                        name: '<%= pkg.name %>',
                        version: '<%= pkg.version %>',
                        author: '<%= pkg.author.name %>',
                        description: '<%= pkg.description %>'
                    }
                },
                files: {
                    'tmp/nuget/<%%= pkg.name %>.nuspec': 'package.nuspec'
                }
            }
        },

        mkdir: {
            pkg: {
                options: {
                    create: ['pkg/nuget', 'pkg/umbraco']
                },
            },
        },

        nugetpack: {
            dist: {
                src: 'tmp/nuget/<%= pkg.name %>.nuspec',
                dest: 'pkg/nuget/'
            }
        },

        umbracoPackage: {
            options: {
                name: '<%= pkg.name %>',
                version: '<%= pkg.version %>',
                url: '<%= pkg.url %>',
                license: '<%= pkg.license %>',
                licenseUrl: '<%= pkg.licenseUrl %>',
                author: '<%= pkg.author %>',
                authorUrl: '<%= pkg.authorUrl %>',
                manifest: 'config/package.xml',
                readme: 'config/readme.txt',
                sourceDir: 'tmp/umbraco',
                outputDir: 'pkg/umbraco',
            }
        },

        clean: {
            dist: '<%= dest %>',
            test: 'src/RJP.MultiUrlPicker/test/assets'
        },

        karma: {
            unit: {
                configFile: 'src/RJP.MultiUrlPicker/test/karma.conf.js'
            }
        },

        jshint: {
            dev: {
                files: {
                    src: ['**/*.js']
                },
                options: {
                    curly: true,
                    eqeqeq: true,
                    immed: true,
                    latedef: true,
                    newcap: true,
                    noarg: true,
                    sub: true,
                    boss: true,
                    eqnull: true,
                    //NOTE: we need to use eval sometimes so ignore it
                    evil: true,
                    //NOTE: we need to check for strings such as "javascript:" so don't throw errors regarding those
                    scripturl: true,
                    //NOTE: we ignore tabs vs spaces because enforcing that causes lots of errors depending on the text editor being used
                    smarttabs: true,
                    globals: {}
                }
            }
        }
    });

    grunt.registerTask('default', ['jshint']);
    grunt.registerTask('nuget', ['clean', 'default', 'copy:nuget', 'template:nuspec', 'mkdir:pkg', 'nugetpack']);
    grunt.registerTask('package', ['clean', 'default', 'copy:umbraco', 'mkdir:pkg', 'umbracoPackage']);

    grunt.registerTask('test', 'Clean, copy test assets, test', function () {
        var assetsDir = grunt.config.get('dest');
        //copies over umbraco assets from --target, this must point at the /umbraco/ directory
        if (assetsDir !== 'dist') {
            grunt.task.run(['clean:test', 'copy:testAssets', 'karma']);
        } else if (grunt.file.isDir('src/RJP.MultiUrlPicker/test/assets/js/')) {
            grunt.log.oklns('Test assets found, running tests');
            grunt.task.run(['karma']);
        } else {
            grunt.log.errorlns('Tests assets not found, skipping tests. Run with --target option to copy assets.');
        }
    });
};

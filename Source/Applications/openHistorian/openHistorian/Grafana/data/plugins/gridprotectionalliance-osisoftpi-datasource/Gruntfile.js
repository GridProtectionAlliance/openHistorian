module.exports = function (grunt) {
  require('load-grunt-tasks')(grunt)

  grunt.loadNpmTasks('grunt-execute')
  grunt.loadNpmTasks('grunt-contrib-clean')
  grunt.loadNpmTasks('grunt-ts')

  grunt.initConfig({
    clean: {
      dist: ['dist'],
      options: {force: true}
    },
    copy: {
      src_to_dist: {
        cwd: 'src',
        expand: true,
        src: ['**/*', '!**/*.js', '!**/*.ts', '!**/*.scss', '!img/*', "!docs/*"],
        dest: 'dist/'
      },
      img_to_dist: {
        cwd: 'src',
        expand: true,
        src: ['img/**/*'],
        dest: 'dist/'
      },
      docs_to_dist: {
        expand: true,
        src: ['docs/**/*'],
        dest: 'dist/'
      },
      pluginDef: {
        expand: true,
        src: [ 'README.md', "LICENSE" ],
        dest: 'dist/'
      }
    },

    watch: {
      rebuild_all: {
        files: ['src/**/*', 'plugin.json'],
        tasks: ['default'],
        options: {spawn: false}
      }
    },
    babel: {
      options: {
        sourceMap: true,
        moduleRoot: 'gridprotectionalliance-osisoftpi-datasource',
        presets: ['es2015']
      },
      dist: {
        options: {
          plugins: ['transform-es2015-modules-systemjs', 'transform-es2015-for-of']
        },
        files: [{
          cwd: 'src',
          expand: true,
          src: ['**/*.js'],
          dest: 'dist',
          ext: '.js'
        }]
      }
    }
  })

  grunt.registerTask('default', ['clean', 'copy:src_to_dist', 'copy:img_to_dist', 'copy:docs_to_dist', 'copy:pluginDef', 'babel'])
}

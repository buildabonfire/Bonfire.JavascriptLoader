'use strict';

import gulp from 'gulp';
import ignite from 'gulp-ignite';
import browserify from 'gulp-ignite-browserify';
import babelify from 'babelify';
import inject from 'gulp-inject';
import uglify from 'uglify-js';

const ASSET_PATH = './src/Bonfire.JavascriptLoader/Assets';
const INJECT_PATH = './src/Bonfire.JavascriptLoader/Core';

const buildTask = {
  name: 'build',
  fn() {
    return gulp.src(INJECT_PATH + '/JavascriptLoaderHtmlHelper.cs')
      .pipe(inject(gulp.src(`${ASSET_PATH}/loader.js`), {
        starttag: '/*INJECT:JS*/',
        endtag: '/*ENDINJECT*/',
        transform: (filepath, file) => {
          return `"${uglify.minify(file.contents.toString('utf8'), { fromString: true }).code.slice(1)}"`;
        }
      }))
      .pipe(gulp.dest(INJECT_PATH));
  }
}

const tasks = [
  browserify,
  buildTask,
];

const options = {
  browserify: {
    src: './src/Bonfire.JavascriptLoader.Demo/Assets/main.js',
    dest: './src/Bonfire.JavascriptLoader.Demo/Content/js',
    options: {
      transform: [babelify],
    },
    watchFiles: [
      './src/Bonfire.JavascriptLoader.Demo/Assets/*',
    ],
  },
};

ignite.start(tasks, options);
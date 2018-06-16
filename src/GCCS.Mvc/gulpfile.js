/// <binding BeforeBuild='sass' />

var gulp = require("gulp"),
    fs = require("fs"),
    autoprefixer = require("autoprefixer"),
    postcss = require("gulp-postcss"),
    sourceMaps = require("gulp-sourcemaps"),
    cssmin = require("gulp-cssmin"),
    rename = require("gulp-rename"),
    sass = require("gulp-sass");

gulp.task("sass:site", function () {
    return gulp.src('Frontend/scss/site.scss')
        .pipe(sass())
        .pipe(postcss([autoprefixer()]))
        .pipe(gulp.dest('wwwroot/css'))
        .pipe(cssmin())
        .pipe(rename('site.min.css'))
        .pipe(gulp.dest('wwwroot/css'));
});

gulp.task("sass:vendor", function () {
    return gulp.src('Frontend/scss/vendor.scss')
        .pipe(sass())
        .pipe(postcss([autoprefixer()]))
        .pipe(gulp.dest('wwwroot/css'))
        .pipe(cssmin())
        .pipe(rename('vendor.min.css'))
        .pipe(gulp.dest('wwwroot/css'));
});

gulp.task("sass", ["sass:site", "sass:vendor"]);
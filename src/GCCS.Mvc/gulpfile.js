/// <binding BeforeBuild='sass' />

var gulp = require("gulp"),
    fs = require("fs"),
    cssmin = require("gulp-cssmin"),
    rename = require("gulp-rename"),
    sass = require("gulp-sass");

gulp.task("sass", function () {
    return gulp.src('wwwroot/css/site.scss')
        .pipe(sass())
        .pipe(gulp.dest('wwwroot/css'))
        .pipe(cssmin())
        .pipe(rename('site.min.css'))
        .pipe(gulp.dest('wwwroot/css'));
});
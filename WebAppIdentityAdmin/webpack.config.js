const path = require('path');
const webpack = require('webpack');
const TerserPlugin = require('terser-webpack-plugin');
const MiniCssExtractPlugin = require('mini-css-extract-plugin');
const CheckerPlugin = require('awesome-typescript-loader').CheckerPlugin;

const OptimizeCssAssetsPlugin = require('optimize-css-assets-webpack-plugin');
module.exports = (env) => {
    const isDevBuild = !(env && env.prod);
    return [{
        mode: isDevBuild ? "development" : "production",
        stats: { modules: false },
        entry: { bundle: './ClientApp/app.tsx' },
        resolve: { extensions: ['.js', '.jsx', '.ts', '.tsx'] },
        output: {
            path: path.join(__dirname, "/wwwroot/dist"),
            filename: '[name].js',
            publicPath: 'dist/',
            library: '[name]'
        },
        module: {
            rules: [
                { test: /\.tsx?$/, include: /ClientApp/, use: 'awesome-typescript-loader?silent=true' },
                {test: /\.(sa|sc|c)ss$/,use: [{loader: MiniCssExtractPlugin.loader},'css-loader','sass-loader',],},             
                { test: /.(png|jpg|jpeg|gif|svg|woff|woff2|eot|ttf)(\?v=\d+\.\d+\.\d+)?$/, use: 'url-loader?limit=25000' },
                { test: /\.jpe?g$|\.ico$|\.gif$|\.png$|\.svg$|\.wav$|\.mp3$/, use: 'file-loader?name=[name].[ext]' }
            ]
        },
        devtool: false,
        plugins: [
            new webpack.DllReferencePlugin({
                context: path.join(__dirname, 'wwwroot/dist'),
                manifest: require('./wwwroot/dist/vendor-manifest.json')
            }),
            new MiniCssExtractPlugin({
                stats: { children: false },
                filename: '[name].css',
                chunkFilename: '[id].css',
            })
        ].concat(isDevBuild ? [
            new CheckerPlugin(),
            new webpack.SourceMapDevToolPlugin({
                filename: '[file].map',
                test: [/\.js$/, /\.jsx$/, /\.tsx$/, /\.ts$/],
                exclude: ['vendor.js', 'vendor'],
                moduleFilenameTemplate: path.relative("/wwwroot/dist", '[resourcePath]')
            })
        ] : []),

        optimization: {
            minimize: true,
            minimizer: isDevBuild ? [] : [new TerserPlugin(), new OptimizeCssAssetsPlugin({})],
        }
    }];
};
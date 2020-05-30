/// <binding />
var HtmlWebpackPlugin = require('html-webpack-plugin');
const path = require("path");
const MiniCssExtractPlugin = require("mini-css-extract-plugin");
const postcssPresetEnv = require("postcss-preset-env");
const devMode = process.env.NODE_ENV !== "production";
module.exports = {
    mode: devMode ? "development" : "production",
    entry: {
        map: ["./src/map.js", "./src/styles/map.scss"],
        mapTester: ["./src/map.js", "./src/tester.js", "./src/mapTester.js", "./src/styles/map.scss", "./src/styles/tester.scss"],
        activityChart: ["./src/activityChart.js", "./src/styles/activityChart.scss"],
        activityChartTester: ["./src/activityChart.js", "./src/tester.js", "./src/activityChartTester.js", "./src/styles/activityChart.scss", "./src/styles/tester.scss"]
    },
    output: {
        path: path.resolve(__dirname, "./dist"),
        filename: "js/[name].js"
    },
    module: {
        // Array of rules that tells Webpack how the modules (output)
        // will be created
        rules: [
            {
                test: /\.(sa|sc)ss$/,
                use: [
                    {
                        loader: MiniCssExtractPlugin.loader
                    },
                    {
                        loader: "css-loader",
                        options: {
                            importLoaders: 2
                        }
                    },
                    {
                        loader: "postcss-loader",
                        options: {
                            ident: "postcss",
                            plugins: devMode
                                ? () => []
                                : () => [
                                    postcssPresetEnv({
                                        // Compile our CSS code to support browsers
                                        // that are used in more than 1% of the
                                        // global market browser share.
                                        // https://github.com/browserslist/browserslist#queries
                                        browsers: [">1%"]
                                    }),
                                    require("cssnano")()
                                ]
                        }
                    },
                    {
                        loader: "sass-loader"
                    }
                ]
            },
            {
                test: /\.css$/,
                use: [
                    MiniCssExtractPlugin.loader,
                    'css-loader',
                    {
                        loader: "postcss-loader",
                        options: {
                            ident: "postcss",
                            plugins: devMode
                                ? () => []
                                : () => [
                                    postcssPresetEnv({
                                        browsers: [">1%"]
                                    }),
                                    require("cssnano")()
                                ]
                        }
                    }
                ]
            },
            {
                test: /\.(png|jpe?g|gif)$/,
                use: [
                    {
                        loader: "file-loader",
                        options: {
                            outputPath: 'images',
                            name() {
                                if (process.env.NODE_ENV === 'development') {
                                    return '[name].[ext]';
                                }

                                return '[contenthash].[ext]';
                            },
                        }
                    }
                ]
            }
        ]
    },
    plugins: [
        new MiniCssExtractPlugin({
            filename: devMode ? "css/[name].css" : "css/[name].min.css",
            chunkFilename: '[id].css'
        }),
        new HtmlWebpackPlugin({
            chunks: ['map'],
            template: 'src/map.html',
            filename: 'map.html'
        }),
        new HtmlWebpackPlugin({
            chunks: ['mapTester'],
            template: 'src/mapTester.html',
            filename: 'mapTester.html'
        }),
        new HtmlWebpackPlugin({
            chunks: ['activityChart'],
            template: 'src/activityChart.html',
            filename: 'activityChart.html'
        }),
        new HtmlWebpackPlugin({
            chunks: ['activityChartTester'],
            template: 'src/activityChartTester.html',
            filename: 'activityChartTester.html'
        })
    ]
};
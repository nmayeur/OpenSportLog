/// <binding ProjectOpened='Watch - Development' />
const path = require("path");
const MiniCssExtractPlugin = require("mini-css-extract-plugin");
const postcssPresetEnv = require("postcss-preset-env");
const devMode = process.env.NODE_ENV !== "production";
module.exports = {
    mode: devMode ? "development" : "production",
    entry: {
        map: ["./src/index.js", "./Styles/site.scss"]
    },
    output: {
        path: path.resolve(__dirname, "dist"),
        publicPath: "/css",
        filename: "js/bundle.js"
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
                                        // global market browser share. You can modify
                                        // the target browsers according to your needs
                                        // by using supported queries.
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
                test: /\.(png|jpe?g|gif)$/,
                use: [
                    {
                        loader: "file-loader",
                        options: {
                            // The image will be named with the original name and
                            // extension
                            name: "[name].[ext]",
                            // Indicates where the images are stored and will use
                            // this path when generating the CSS files.
                            // Example, in site.scss I have
                            // url('../wwwroot/images/pattern.png') and when generating
                            // the CSS file, file-loader will output as
                            // url(../images/pattern.png), which is relative
                            // to '/css/site.css'
                            publicPath: "../images",
                            // When this option is 'true', the loader will emit the
                            // image to output.path
                            emitFile: false
                        }
                    }
                ]
            },
            {
                test: /\.css$/,
                use: [
                    'style-loader',
                    'css-loader',
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
                                        // global market browser share. You can modify
                                        // the target browsers according to your needs
                                        // by using supported queries.
                                        // https://github.com/browserslist/browserslist#queries
                                        browsers: [">1%"]
                                    }),
                                    require("cssnano")()
                                ]
                        }
                    }
                ]
            }
        ]
    },
    plugins: [
        // Configuration options for MiniCssExtractPlugin. Here I'm only
        // indicating what the CSS output file name should be and
        // the location
        new MiniCssExtractPlugin({
            filename: devMode ? "css/site.css" : "css/site.min.css"
        })
    ]
};
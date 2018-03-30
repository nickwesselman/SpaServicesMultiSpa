# SpaServices Multi-SPA Example w/ Angular CLI

What is working:

* Multi-SPA setup with independent Angular apps
* Running both apps at separate paths in the .NET Core host in Development mode (proxying to Angular CLI server)
* The same, in Production mode (running from a build in dist)
* Live reload in Development mode
* SSR / prerender for both apps
* Debugging SSR on an incrementing port

What's not working:

* Source maps for SSR debugging (https://github.com/angular/angular-cli/issues/8931)

Key points in the Angular CLI / build config for both apps:

* `baseHref` is set in the ng CLI config – this ensures each app runs correctly at its intended path on the host. On angular-cli 1.7.0 it appears that ng serve will launch the app on this path as well, allowing you to run the app directly with the same pathing.
* The apps need an npm script specifically for running in Development mode (`start:hosted`), to ensure proper file pathing and to change the live reload endpoint to account for the app pathing/proxying
* Live reloading fixed via the `live-reload-client` argument on `ng serve`
* Production build just requires `npm run build` sice `baseHref` is configured in `.angular-cli.json`

The real work here on the .NET side is in `SpaPluginExtensions.PluginSpa`.

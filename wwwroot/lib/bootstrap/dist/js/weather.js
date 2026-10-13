window.echovibeGetLocation = () => {
    return new Promise((resolve) => {
        if (!navigator.geolocation) {
            resolve({ ok: false });
            return;
        }
        navigator.geolocation.getCurrentPosition(
            (pos) => resolve({
                ok: true,
                latitude: pos.coords.latitude,
                longitude: pos.coords.longitude
            }),
            () => resolve({ ok: false }),
            { enableHighAccuracy: false, timeout: 8000, maximumAge: 600000 }
        );
    });
};
import React, { useEffect, useState } from 'react';
import { useUnit } from 'effector-react';
import type { LatLngTuple } from 'leaflet';

import { useFormattedTime } from './useFormattedTime';
import { timezoneModel } from './timezone.model';

export const useTimezoneAndLocalTime = (point: LatLngTuple, id: string) => {
    const timezone = useUnit(timezoneModel.$savedTimezones)[id];
    const localTime = useFormattedTime(timezone);

    useEffect(() => {
        timezoneModel.getTimezoneFx({ anyId: id, lat: point[0], long: point[1] });
    }, []);

    return { timezone, localTime };
};
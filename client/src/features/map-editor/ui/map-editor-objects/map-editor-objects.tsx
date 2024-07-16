import React, { useEffect } from 'react';
import { useUnit } from 'effector-react';
import { Circle, Polyline, Polygon } from 'react-leaflet';
import type { LeafletMouseEvent } from 'leaflet';

import { mapModel } from '../../../../entities/map';

import { selectedOptions, defaultOptions } from '../../lib/constants';
import { editorModel } from '../../lib/editor.model';
import { MapEditorPopupForm } from '../map-editor-popup-form/map-editor-popup-form';

/** Рендерит геообьекты на карте */
export const MapEditorObjects = () => {
    const $map = useUnit(mapModel.$map);

    const $points = useUnit(editorModel.$points);
    const $lines = useUnit(editorModel.$lines);
    const $polygons = useUnit(editorModel.$polygons);

    useEffect(() => {
        if (!$map) {
            return;
        }

        const handleMapClick = (e: LeafletMouseEvent) => {
            editorModel.addPoint([e.latlng.lat, e.latlng.lng]);
        };

        $map.addEventListener('click', handleMapClick);

        return () => {
            $map.removeEventListener('click', handleMapClick);
        };
    }, [$map]);

    return (
        <>
            {Object.values($points).map(({ id, coordinates, selected }) => (
                <Circle
                    key={id}
                    center={coordinates}
                    pathOptions={selected ? selectedOptions : defaultOptions}
                    radius={20}
                    eventHandlers={{
                        click: () => editorModel.togglePointSelect(id),
                        mouseover: (e) => e.target.openPopup(),
                    }}
                >
                    <MapEditorPopupForm id={id} type='point' onDelete={() => editorModel.deletePoint(id)} />
                </Circle>
            ))}

            {Object.values($lines).map(({ id, points, selected }) => (
                <Polyline
                    weight={7}
                    key={id}
                    positions={points.map(({ coordinates }) => coordinates)}
                    pathOptions={selected ? selectedOptions : defaultOptions}
                    eventHandlers={{
                        click: () => editorModel.toggleLineSelect(id),
                        mouseover: (e) => e.target.openPopup(),
                    }}
                >
                    <MapEditorPopupForm id={id} type='line' onDelete={() => editorModel.deleteLine(id)} />
                </Polyline>
            ))}

            {Object.values($polygons).map(({ id, points, selected }) => (
                <Polygon
                    key={id}
                    positions={points.map(({ coordinates }) => coordinates)}
                    pathOptions={selected ? selectedOptions : defaultOptions}
                    eventHandlers={{
                        click: () => editorModel.togglePolygonSelect(id),
                        mouseover: (e) => e.target.openPopup(),
                       /*  mouseout: (e) => e.target.closePopup(), */
                    }}
                >
                    <MapEditorPopupForm id={id}  type='polygon' /* polygonId={id} */ onDelete={() => editorModel.deletePolygon(id)} />
                </Polygon>
            ))}
        </>
    );
};

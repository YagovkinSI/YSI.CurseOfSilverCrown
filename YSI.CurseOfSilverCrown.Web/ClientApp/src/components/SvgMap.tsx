import * as React from 'react';
import SvgMapImage from './SvgMapImage';
import getFeods from './Feods';
import IDomainPublic from '../apiModels/domainPublic';
import SvgMapElement from './SvgMapElement';

interface ISvgMapProps {
    domainPublicList: IDomainPublic[],
    onClickDomain: (id: number) => void
}

const SvgMap: React.FC<ISvgMapProps> = (props) => {
    const feods = getFeods();

    return (
        <svg version="1.1" viewBox="0 0 1920 2887" xmlns="http://www.w3.org/2000/svg">
            <g transform="translate(0 .075438)" style={{ strokeWidth: 1.0016 }}>
                <SvgMapImage />
                {props.domainPublicList.map(domain => {
                    const feod = feods.find(f => f.id == domain.id);
                    if (feod != undefined)
                        return <SvgMapElement
                            key={domain.id}
                            domainPublic={domain}
                            path={feod.path}
                            onClickDomain={props.onClickDomain}
                        />
                })}
            </g>
        </svg>
    )
};

export default SvgMap;
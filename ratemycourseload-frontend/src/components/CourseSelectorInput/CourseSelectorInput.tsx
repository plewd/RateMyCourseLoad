import { Course } from '@types'
import { Autocomplete, Grid } from '@mui/joy'
import ClassIcon from '@mui/icons-material/Class'
import { useState } from 'react'

interface Props {
    coursePrefixOptions: string[]
    onSelected: (selected: Course) => void
    disabled: boolean
}

// const findPrefix = (course: Course) => {
//     const courseName = course.Name
//     for (let i = 0; i < courseName.length; i++) {
//         const c = courseName[i]
//         if (c >= '0' && c <= '9') {
//             return courseName.substring(0, i)
//         }
//     }
//     throw new Error(`Can't find prefix for course: ${courseName}`)
// }

function CourseSelectorInput({
    coursePrefixOptions,
    // onSelected,
    disabled,
}: Props) {
    const [coursePrefix, setCoursePrefix] = useState<string>('')
    const [courseNumber, setCourseNumber] = useState<string>('')

    return (
        <Grid container>
            <Grid xs={6}>
                <Autocomplete
                    onChange={(_event, value) => {
                        if (value !== null) {
                            setCoursePrefix(value)
                            // onSelected(value)
                        }
                    }}
                    value={coursePrefix}
                    blurOnSelect={true}
                    placeholder="Prefix"
                    // groupBy={findPrefix}
                    disabled={disabled}
                    variant="soft"
                    startDecorator={<ClassIcon />}
                    // getOptionLabel={(option) => option.Name}
                    options={coursePrefixOptions.sort()}
                />
            </Grid>

            <Grid xs={6}>
                <Autocomplete
                    style={{ marginLeft: '5px' }}
                    onChange={(_event, value) => {
                        if (value !== null) {
                            setCourseNumber(value)
                            // onSelected(value)
                        }
                    }}
                    value={courseNumber}
                    blurOnSelect={true}
                    placeholder="Number"
                    // groupBy={findPrefix}
                    disabled={coursePrefix === ''}
                    variant="soft"
                    // getOptionLabel={(option) => option.Name}
                    options={['2500', '3999', '4000']}
                />
            </Grid>
        </Grid>
    )
}

export default CourseSelectorInput

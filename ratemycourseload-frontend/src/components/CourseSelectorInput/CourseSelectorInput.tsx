import { Course } from '@types'
import { Autocomplete } from '@mui/joy'
import ClassIcon from '@mui/icons-material/Class'

interface Props {
    courseOptions: Course[]
    onSelected: (selected: Course) => void
    disabled: boolean
}

const findPrefix = (course: Course) => {
    const courseName = course.Name
    for (let i = 0; i < courseName.length; i++) {
        const c = courseName[i]
        if (c >= '0' && c <= '9') {
            return courseName.substring(0, i)
        }
    }
    throw new Error(`Can't find prefix for course: ${courseName}`)
}

function CourseSelectorInput({ courseOptions, onSelected, disabled }: Props) {
    return (
        <Autocomplete
            onChange={(_event, value) => {
                if (value !== null) {
                    onSelected(value)
                }
            }}
            value={null}
            blurOnSelect={true}
            placeholder="Choose a course"
            groupBy={findPrefix}
            disabled={disabled}
            variant="soft"
            startDecorator={<ClassIcon />}
            getOptionLabel={(option) => option.Name}
            options={courseOptions.sort((c1, c2) =>
                c1.Name.localeCompare(c2.Name)
            )}
        />
    )
}

export default CourseSelectorInput

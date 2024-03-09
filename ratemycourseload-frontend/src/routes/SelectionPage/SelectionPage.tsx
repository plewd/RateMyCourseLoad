import { Course } from '@types'
import CourseSelectorInput from '@components/CourseSelectorInput/CourseSelectorInput.tsx'
import { useState } from 'react'
import { Stack, Chip, Button, Divider } from '@mui/joy'
import SelectedCourseList from '@components/SelectedCourseList.tsx'

interface Props {
    coursePrefixOptions: string[]
    onSubmit: (courses: Course[]) => void
}

const maxNumberCourses = 6

export default function SelectionPage({
    coursePrefixOptions,
    onSubmit,
}: Props) {
    const [selectedCourses, setSelectCourses] = useState<Course[]>([])
    const isAtMaxCourses = selectedCourses.length >= maxNumberCourses
    // const [responseLoading, setResponseLoading] = useState<boolean>(false)

    const removeCourse = (courseName: string) => {
        setSelectCourses(
            selectedCourses.filter((course) => course.Name !== courseName)
        )
    }

    const creditHours = selectedCourses.reduce(
        (acc, curr) => acc + curr.CreditHours,
        0
    )

    return (
        <>
            <Stack spacing={3}>
                <SelectedCourseList
                    selectedCourses={selectedCourses}
                    removeCourse={removeCourse}
                    showDelete
                />

                {selectedCourses.length > 0 && (
                    <Chip>{creditHours} Credit Hours</Chip>
                )}

                <CourseSelectorInput
                    coursePrefixOptions={coursePrefixOptions}
                    disabled={isAtMaxCourses}
                    onSelected={(course) => {
                        setSelectCourses([...selectedCourses, course])
                    }}
                />

                {selectedCourses.length > 0 && (
                    <>
                        <Divider />
                        <Button
                            onClick={() => {
                                onSubmit(selectedCourses)
                            }}
                        >
                            Rate my course load
                        </Button>
                    </>
                )}
            </Stack>
        </>
    )
}

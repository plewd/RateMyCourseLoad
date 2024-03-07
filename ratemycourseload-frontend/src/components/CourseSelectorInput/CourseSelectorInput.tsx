import { Course } from '@types'
import {
    Autocomplete,
    Button,
    CircularProgress,
    FormHelperText,
    Grid,
} from '@mui/joy'
import ClassIcon from '@mui/icons-material/Class'
import PublishIcon from '@mui/icons-material/Publish'
import React, { useState } from 'react'

interface Props {
    coursePrefixOptions: string[]
    onSelected: (selected: Course) => void
    disabled: boolean
}

type CourseSuggestion = {
    prefix: string
    number: string
    title: string
    description: string
    creditHours: number
}

function CourseSelectorInput({
    coursePrefixOptions,
    onSelected,
    disabled,
}: Props) {
    const [coursePrefix, setCoursePrefix] = useState<string | null>(null)
    const [course, setCourse] = useState<CourseSuggestion | null>(null)
    const [loading, setLoading] = useState<boolean>(false)

    const [courseSuggestions, setCourseSuggestions] = useState<
        CourseSuggestion[] | null
    >(null)

    const fetchCourseSuggestions = async (coursePrefix: string) => {
        try {
            setLoading(true)
            const response = await fetch(
                `http://localhost:5000/api/course/prefix/${coursePrefix}`
            )
            if (!response.ok) {
                throw new Error('Network response was not ok')
            }
            const data: CourseSuggestion[] = await response.json()
            setCourseSuggestions(data)
            setLoading(false)
        } catch (error) {
            console.error('Error:', error)
            alert('womp...')
        }
    }

    /*
    event: React.SyntheticEvent,     value: Value | Value[]
     */
    const coursePrefixSet = (
        _event: React.SyntheticEvent,
        value: string | null
    ) => {
        if (value !== null) {
            setCoursePrefix(value)
            setCourse(null)
            void fetchCourseSuggestions(value)
        } else {
            setCoursePrefix(null)
            setCourse(null)
        }
    }

    const isCourseReadyToAdd = coursePrefix !== null && course !== null

    return (
        <Grid container>
            <Grid xs={6}>
                <Autocomplete
                    variant="soft"
                    autoSelect
                    autoHighlight
                    clearOnBlur
                    blurOnSelect
                    onChange={coursePrefixSet}
                    value={coursePrefix}
                    placeholder="Prefix"
                    disabled={disabled}
                    startDecorator={<ClassIcon />}
                    options={coursePrefixOptions.sort()}
                />
            </Grid>

            <Grid xs={5} style={{ paddingLeft: '5px' }}>
                <Autocomplete
                    variant="soft"
                    autoSelect
                    autoHighlight
                    clearOnBlur
                    blurOnSelect
                    onChange={(_event, value) => {
                        if (value !== null) {
                            setCourse(value)
                        }
                    }}
                    value={course}
                    placeholder={loading ? '' : 'Number'}
                    disabled={loading || coursePrefix === null}
                    getOptionLabel={(option) => option.number}
                    options={
                        courseSuggestions === null ? [] : courseSuggestions
                    }
                    endDecorator={
                        loading && (
                            <CircularProgress
                                size="sm"
                                sx={{ bgcolor: 'background.surface' }}
                            />
                        )
                    }
                />
            </Grid>
            <Grid xs={1} style={{ paddingLeft: '5px' }}>
                <Button
                    disabled={!isCourseReadyToAdd}
                    onClick={() => {
                        if (course !== null) {
                            onSelected({
                                Name: `${course.prefix}${course.number}`,
                                CreditHours: course.creditHours,
                                Title: course.title,
                            })
                            setCoursePrefix(null)
                            setCourse(null)
                            setCourseSuggestions(null)
                        }
                    }}
                >
                    <PublishIcon />
                </Button>
            </Grid>
            {isCourseReadyToAdd && (
                <Grid xs={12} style={{ paddingTop: '10px' }}>
                    <FormHelperText>{course.title}</FormHelperText>
                </Grid>
            )}
            {disabled && (
                <FormHelperText style={{ paddingTop: '10px' }}>
                    The maximum amount of courses have been added
                </FormHelperText>
            )}
        </Grid>
    )
}

export default CourseSelectorInput
